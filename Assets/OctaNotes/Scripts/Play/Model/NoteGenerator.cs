using System;
using System.Collections.Generic;
using System.Linq;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.ViewModel;
using OctaNotes.Scripts.Settings;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class NoteGenerator : MonoBehaviour
    {
        [Inject] private readonly PlaySettingsSO playsettingsSO;
        [Inject] private IChartRepositoryImmutable _chartRepository;
        
        [SerializeField] private GameObject tapNotePrefab;
        [SerializeField] private GameObject longNotePrefab;
        [SerializeField] private GameObject chainNotePrefab;
        [SerializeField] private GameObject supportPlanePrefab;
        [SerializeField] private GameObject supportLinePrefab;
        [SerializeField] private Material supportPlaneMaterial;
        
        private double zOffset = 0; // ノーツ生成のZオフセット


        private Dictionary<int, double> laneXPositions = new Dictionary<int, double>()
        {
            { 0, -1.5 },
            { 1, -0.5 },
            { 2, 0.5 },
            { 3, 1.5 },
            { 4, -1.5 },
            { 5, -0.5 },
            { 6, 0.5 },
            { 7, 1.5 }
        };
        
        private float noteSpeed = 1.0f; // ノーツの移動速度（例）

        private void Start()
        {
            noteSpeed = (float)playsettingsSO.noteSpeed;
            zOffset = playsettingsSO.songStartDelay * noteSpeed;
            Debug.Log(zOffset);
            Generate();
        }

        private void Generate()
        {
            double[] longStartTmpPos = new double[8]{ double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
            char[] longNoteColor = new char[8]{ 'b', 'b', 'b', 'b', 'b', 'b', 'b', 'b' };
            foreach (var rawZPos in _chartRepository.GraphicalChartData.Keys.ToList())
            {
                var noteTypes = _chartRepository.GraphicalChartData[rawZPos];
                double zPos = rawZPos;
                var tapnoteLaneIdx =
                    noteTypes.Select((value, index) => new { value, index })
                        .Where(x =>!string.IsNullOrEmpty(x.value) && (x.value.StartsWith("b") || x.value.StartsWith("r") || x.value.StartsWith("lb") || x.value.StartsWith("lr") || x.value.StartsWith("lw")))
                        .Select(x => x.index)
                        .ToList();
                
                List<bool> tapnoteLaneFlag = new List<bool>();
                for (int i = 0; i < 8; i++)
                {
                    tapnoteLaneFlag.Add(tapnoteLaneIdx.Contains(i));
                }
                
                GenerateSupportPlane(tapnoteLaneFlag, zPos); // 補助線・補助面の生成
                // 各タイミングレーンごとに処理していく
                for (int lane = 0; lane < 8; lane++)
                {
                    var noteType = noteTypes[lane];
                    if (string.IsNullOrEmpty(noteType)) continue;
                    
                    
                    if (noteType.StartsWith("b") || noteType.StartsWith("r") || noteType.StartsWith("w"))
                    {
                        // タップノーツ
                        GenerateTapNote(lane, zPos, noteType[0]);
                    }
                    else if (noteType == "le")
                    {
                         // ロングノーツ終了
                        if (!double.IsNaN(longStartTmpPos[lane]))
                        { 
                            GenerateLongNote(lane, longStartTmpPos[lane], zPos, longNoteColor[lane]);
                            longStartTmpPos[lane] = double.NaN;
                        }
                    }
                    else if (noteType.StartsWith("l"))
                    {
                        // ロングノーツ開始
                        longStartTmpPos[lane] = zPos;
                        GenerateTapNote(lane, zPos, noteType[1]);
                        longNoteColor[lane] = noteType[1];
                    }
                    
                }
            }
        }
        
        

        private void GenerateTapNote(int lane, double z, char noteColor='b')
        {
            var x = laneXPositions[lane];
            float y = (lane < 4) ? 0.01f : 1.99f; // 上段と下段でy座標を分ける例
            var rotation = Quaternion.Euler((lane < 4)?0:180, 0f, 0f);
            float posZ = (float)(z * noteSpeed + zOffset);
            var note =Instantiate(tapNotePrefab, new Vector3((float)x, y, posZ), rotation);
            note.GetComponent<INoteViewModel>().SetInitialPosZ(posZ);
            SetNoteColor(note.GetComponent<MeshRenderer>(), noteColor);
        }
        
        private void GenerateLongNote(int lane, double startZ, double endZ, char noteColor='b')
        {
            var x = laneXPositions[lane];
            float y = (lane < 4) ? 0.009f : 1.991f ; // 上段と下段でy座標を分ける例
            var rotation = Quaternion.Euler((lane < 4)?0:180, 0f, 0f);
            float startPosZ = (float)(startZ * noteSpeed + zOffset);
            var longNote = Instantiate(longNotePrefab, new Vector3((float)x, y, startPosZ), rotation);
            longNote.transform.localScale = new Vector3(1,1, ((lane < 4)?1:-1) * (float)(endZ - startZ)*noteSpeed);
            longNote.GetComponent<INoteViewModel>().SetInitialPosZ(startPosZ);
            SetNoteColor(longNote.GetComponent<LongNoteRendererRef>().meshRenderer, noteColor);
        }

        private void GenerateSupportPlane(List<bool> noteLaneFlag, double z)
        {
            // 0,1,2,3,7,6,5,4の順番に走査して、連続するノーツレーンをまとめる
            List<int> lanes = new List<int>();
            for (int i = 0; i < 8; i++)
            {
                int laneIdx = (i < 4) ? i : 11 - i;
                if (noteLaneFlag[laneIdx])
                {
                    lanes.Add(laneIdx);
                }
            }
            
            float posZ = (float)(z * noteSpeed + zOffset);
            
            if (lanes.Count() == 2) // 2個の場合は補助面ではなく線を生成
            {
                // var line = new GameObject("SupportLine", typeof(LineRenderer));
                var line = Instantiate(supportLinePrefab);
                line.GetComponent<INoteViewModel>().SetInitialPosZ(posZ);
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, new Vector3((float)laneXPositions[lanes[0]], (lanes[0] < 4) ? 0.009f : 1.991f, posZ));
                lineRenderer.SetPosition(1, new Vector3((float)laneXPositions[lanes[1]], (lanes[1] < 4) ? 0.009f : 1.991f, posZ));
                lineRenderer.startWidth = 0.02f;
                lineRenderer.endWidth = 0.02f;
            }
            else if (lanes.Count() >= 3)
            {
                var mesh = GenerateMesh(lanes.Select(lane => new Vector3((float)laneXPositions[lane], (lane < 4) ? 0.009f : 1.991f, posZ)).ToList());
                // var supportPlane = new GameObject("SupportPlane", typeof(MeshFilter), typeof(MeshRenderer));
                var supportPlane = Instantiate(supportPlanePrefab);
                supportPlane.GetComponent<MeshFilter>().mesh = mesh;
            }
        }
        
        private Mesh GenerateMesh(IList<Vector3> inputVertices)
        {
            if (inputVertices == null || inputVertices.Count < 3)
                throw new System.ArgumentException("頂点は3つ以上必要です");

            // 1. 平面法線（向きはどうでもいい）
            Vector3 normal = ComputeNormal(inputVertices).normalized;
            normal = new Vector3(Mathf.Abs(normal.x), Mathf.Abs(normal.y), Mathf.Abs(normal.z));

            // 2. 重心
            Vector3 center = Vector3.zero;
            foreach (var v in inputVertices)
                center += v;
            center /= inputVertices.Count;

            // 3. 捨てる軸を決定（最も法線が強い軸）
            int dropAxis;
            if (normal.x >= normal.y && normal.x >= normal.z)
                dropAxis = 0; // Xを捨てる → YZ平面
            else if (normal.y >= normal.z)
                dropAxis = 1; // Yを捨てる → XZ平面
            else
                dropAxis = 2; // Zを捨てる → XY平面

            // 4. 角度ソート（2D）
            var sorted = new List<Vector3>(inputVertices);
            sorted.Sort((a, b) =>
            {
                Vector2 da = Project2D(a - center, dropAxis);
                Vector2 db = Project2D(b - center, dropAxis);

                float angleA = Mathf.Atan2(da.y, da.x);
                float angleB = Mathf.Atan2(db.y, db.x);

                return angleA.CompareTo(angleB);
            });

            // 5. Triangle Fan
            var triangles = new List<int>();
            for (int i = 1; i < sorted.Count - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }

            // 6. Mesh
            Mesh mesh = new Mesh();
            mesh.SetVertices(sorted);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static Vector2 Project2D(Vector3 v, int dropAxis)
        {
            return dropAxis switch
            {
                0 => new Vector2(v.y, v.z), // drop X
                1 => new Vector2(v.x, v.z), // drop Y
                _ => new Vector2(v.x, v.y), // drop Z
            };
        }
        
        private Vector3 ComputeNormal(IList<Vector3> vertices)
        {
            Vector3 normal = Vector3.zero;

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 current = vertices[i];
                Vector3 next = vertices[(i + 1) % vertices.Count];

                normal.x += (current.y - next.y) * (current.z + next.z);
                normal.y += (current.z - next.z) * (current.x + next.x);
                normal.z += (current.x - next.x) * (current.y + next.y);
            }

            return normal.normalized;
        }

        private void SetNoteColor(MeshRenderer meshRenderer, char color)
        {
            var mat = meshRenderer.material;
            switch (color)
            {
                case 'b':
                    mat.EnableKeyword("_COLOR_BLUE");
                    mat.DisableKeyword("_COLOR_RED");
                    mat.DisableKeyword("_COLOR_EX");
                    break;
                case 'r':
                    mat.EnableKeyword("_COLOR_RED");
                    mat.DisableKeyword("_COLOR_BLUE");
                    mat.DisableKeyword("_COLOR_EX");
                    break;
                default:
                    mat.EnableKeyword("_COLOR_EX");
                    mat.DisableKeyword("_COLOR_BLUE");
                    mat.DisableKeyword("_COLOR_RED");
                    break;
            }
        }
    }
}
