using System;
using System.Collections.Generic;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.DI.Lane;
using OctaNotes.Scripts.Play.Interface;
using OctaNotes.Scripts.Play.ViewModel;
using OctaNotes.Scripts.Play.ViewModel.Interface;
using OctaNotes.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace OctaNotes.Scripts.Play.Model
{
    public class NoteGenerator : MonoBehaviour
    {
        private PlaySettingsSO _playsettingsSO;
        private IChartRepositoryImmutable _chartRepository;
        private ILaneSubContainerFactory  _laneSubContainerFactory;
        
        

        [Inject]
        public void Construct(PlaySettingsSO playsettingsSO, IChartRepositoryImmutable chartRepository,  ILaneSubContainerFactory laneSubContainerFactory)
        {
            _playsettingsSO = playsettingsSO;
            _chartRepository = chartRepository;
            _laneSubContainerFactory = laneSubContainerFactory;
        }
        
        
        [SerializeField] private GameObject tapNotePrefab;
        [SerializeField] private GameObject longNotePrefab;
        [SerializeField] private GameObject chainNotePrefab;
        [SerializeField] private GameObject longEndPrefab;
        [SerializeField] private GameObject supportPlanePrefab;
        [SerializeField] private GameObject supportLinePrefab;
        [SerializeField] private GameObject topSupportPlanePrefab;
        [SerializeField] private Material supportPlaneMaterial;
        
        private double zOffset = 0; // ノーツ生成のZオフセット
        private readonly bool[] _tapnoteLaneFlagBuffer = new bool[8];
        private readonly Guid[] _guidsBuffer = new Guid[8];
        private readonly double[] _laneXPositionCache = { -1.5, -0.5, 0.5, 1.5, -1.5, -0.5, 0.5, 1.5 };
        private readonly float[] _laneYPositionCache = { -0.1f, -0.1f, -0.1f, -0.1f, 2.1f, 2.1f, 2.1f, 2.1f };
        private static readonly Quaternion[] LaneRotationCache =
        {
            Quaternion.Euler(0f, 0f, 0f),
            Quaternion.Euler(0f, 0f, 0f),
            Quaternion.Euler(0f, 0f, 0f),
            Quaternion.Euler(0f, 0f, 0f),
            Quaternion.Euler(180f, 0f, 0f),
            Quaternion.Euler(180f, 0f, 0f),
            Quaternion.Euler(180f, 0f, 0f),
            Quaternion.Euler(180f, 0f, 0f),
        };
        private static readonly int[] SupportLaneOrder = { 0, 1, 2, 3, 7, 6, 5, 4 };


        private readonly float bottonYPosition = -0.1f;
        private readonly float topYPosition = 2.1f;
        
        private float noteSpeed = 1.0f; // ノーツの移動速度（例）

        private void Start()
        {
            noteSpeed = (float)_playsettingsSO.noteSpeed;
            // zOffset = _playsettingsSO.songStartDelay * noteSpeed;
            Debug.Log(zOffset);
            Generate();
        }

        private void Generate()
        {
            var longStartTmpPos = new double[8]{ double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN };
            var longNoteColor = new NoteColor[8] { NoteColor.Blue , NoteColor.Blue, NoteColor.Blue, NoteColor.Blue, NoteColor.Blue, NoteColor.Blue, NoteColor.Blue, NoteColor.Blue};
            var laneContainerCache = new Zenject.DiContainer[8];
            for (var lane = 0; lane < 8; lane++)
            {
                laneContainerCache[lane] = _laneSubContainerFactory.GetLaneSubContainer(lane);
            }

            foreach (var chartEntry in _chartRepository.GraphicalChartData)
            {
                var zPos = chartEntry.Key;
                var noteEntries = chartEntry.Value;

                for (var i = 0; i < 8; i++)
                {
                    _tapnoteLaneFlagBuffer[i] = false;
                    _guidsBuffer[i] = Guid.Empty;
                }

                // タイミングごとに各レーンに存在するノーツを処理していく
                for (var lane = 0; lane < 8; lane++)
                {
                    var noteEntry = noteEntries[lane];
                    var noteGuid = noteEntry.guid;
                    _guidsBuffer[lane] = noteGuid;

                    if (noteEntry.noteType is NoteType.Tap or NoteType.LongStart or NoteType.Chain)
                    {
                        _tapnoteLaneFlagBuffer[lane] = true;
                    }

                    switch (noteEntry.noteType)
                    {
                        case NoteType.Tap:
                            GenerateTapNote(laneContainerCache[lane], lane, zPos,noteGuid, noteEntry.noteColor);
                            break;
                        case NoteType.Chain:
                            GenerateChainNote(laneContainerCache[lane], lane, zPos, noteGuid, noteEntry.noteColor);
                            break;
                        case NoteType.LongStart:
                            longStartTmpPos[lane] = zPos;
                            GenerateTapNote(laneContainerCache[lane], lane, zPos, noteGuid, noteEntry.noteColor);
                            longNoteColor[lane] = noteEntry.noteColor;
                            break;
                        case NoteType.LongEnd:
                            if (!double.IsNaN(longStartTmpPos[lane]))
                            { 
                                GenerateLongNote(laneContainerCache[lane], lane, longStartTmpPos[lane], zPos, noteGuid, longNoteColor[lane]);
                                GenerateLongEnd(laneContainerCache[lane], lane, zPos,  noteGuid, longNoteColor[lane]);
                                longStartTmpPos[lane] = double.NaN;
                            }
                            break;
                        case NoteType.None:
                            break;
                    }
                    if (lane >= 4 && noteEntry.noteType is NoteType.Tap or NoteType.LongStart or NoteType.Chain)
                    {
                        GenerateTopSupport(laneContainerCache[lane], lane, zPos, noteGuid);
                    }
                }
                GenerateSupportPlane(_tapnoteLaneFlagBuffer, zPos, _guidsBuffer, laneContainerCache[0]); // 補助線・補助面の生成

            }
        }
        
        

        private void GenerateTapNote(Zenject.DiContainer laneContainer, int lane, double z, Guid noteGuid, NoteColor noteColor)
        {
            var x = _laneXPositionCache[lane];
            var y = _laneYPositionCache[lane];
            var rotation = LaneRotationCache[lane];
            float posZ = (float)(z * noteSpeed + zOffset);
            
            var note = laneContainer.InstantiatePrefab(tapNotePrefab);
            note.transform.position =  new Vector3((float)x, y, posZ);
            note.transform.rotation = rotation;
            
            var vm = note.GetComponent<GameObjectContext>().Container.Resolve<INoteViewModel>();
            vm.SetInitialPosZ(posZ);
            vm.SetGuid(noteGuid);
            
            SetNoteColor(note.GetComponent<MeshRenderer>(), noteColor);
        }

        private void GenerateChainNote(Zenject.DiContainer laneContainer, int lane, double z, Guid noteGuid, NoteColor noteColor)
        {
            var x = _laneXPositionCache[lane];
            var y = _laneYPositionCache[lane];
            var rotation = LaneRotationCache[lane];
            float posZ = (float)(z * noteSpeed + zOffset);
            
            var note = laneContainer.InstantiatePrefab(chainNotePrefab);
            note.transform.position =  new Vector3((float)x, y, posZ);
            note.transform.rotation = rotation;

            var vm = note.GetComponent<GameObjectContext>().Container.Resolve<INoteViewModel>();
            vm.SetInitialPosZ(posZ);
            vm.SetGuid(noteGuid);
            
            SetNoteColor(note.GetComponent<MeshRenderer>(), noteColor);
        }

        private void GenerateLongEnd(Zenject.DiContainer laneContainer, int lane, double z, Guid noteGuid, NoteColor noteColor)
        {
            var x = _laneXPositionCache[lane];
            var y = _laneYPositionCache[lane];
            var rotation = LaneRotationCache[lane];
            float posZ = (float)(z * noteSpeed + zOffset);
            
            var note = laneContainer.InstantiatePrefab(longEndPrefab);
            note.transform.position =  new Vector3((float)x, y, posZ);
            note.transform.rotation = rotation;
            
            var vm = note.GetComponent<GameObjectContext>().Container.Resolve<INoteViewModel>();
            vm.SetInitialPosZ(posZ);
            vm.SetGuid(noteGuid);
            
            SetNoteColor(note.GetComponent<MeshRenderer>(), noteColor);
        }
        
        
        private void GenerateLongNote(Zenject.DiContainer laneContainer, int lane, double startZ, double endZ, Guid noteGuid, NoteColor noteColor)
        {
            var x = _laneXPositionCache[lane];
            float y = (lane < 4) ? bottonYPosition - 0.001f : topYPosition + 0.001f ; // 上段と下段でy座標を分ける例
            var rotation = LaneRotationCache[lane];
            float startPosZ = (float)(startZ * noteSpeed + zOffset);
            
            var longNote = laneContainer.InstantiatePrefab(longNotePrefab);
            longNote.transform.position =  new Vector3((float)x, y, startPosZ);
            longNote.transform.rotation = rotation;
            longNote.transform.localScale = new Vector3(1,1, ((lane < 4)?1:-1) * (float)(endZ - startZ)*noteSpeed);
            
            var vm = longNote.GetComponent<GameObjectContext>().Container.Resolve<ILongNoteViewModel>();
            laneContainer.Inject(vm); // ノーツごとのViewModelにレーンのSubContainerからDI
            vm.SetInitialPosZ(startPosZ);
            
            SetNoteColor(longNote.GetComponent<LongNoteRendererRef>().meshRenderer, noteColor);
        }

        private void GenerateTopSupport(Zenject.DiContainer laneContainer, int lane, double startZ, Guid noteGuid)
        {
            
            var x = _laneXPositionCache[lane];
            float y = 1;
            float startPosZ = (float)(startZ * noteSpeed + zOffset);
            
            var longNote = laneContainer.InstantiatePrefab(topSupportPlanePrefab);
            longNote.transform.position =  new Vector3((float)x, y, startPosZ);
            
            var vm = longNote.GetComponent<GameObjectContext>().Container.Resolve<ISupportLineViewModel>();
            laneContainer.Inject(vm); // ノーツごとのViewModelにレーンのSubContainerからDI
            vm.SetInitialPosZ(startPosZ);
        }

        private void GenerateSupportPlane(bool[] noteLaneFlag, double z, Guid[] guids, Zenject.DiContainer supportContainer)
        {
            // 頂点を反時計回りに並べたいので、0,1,2,3,7,6,5,4の順番に走査する
            var lanes = new List<int>(8);
            for (var i = 0; i < SupportLaneOrder.Length; i++)
            {
                var laneIdx = SupportLaneOrder[i];
                if (noteLaneFlag[laneIdx])
                {
                    lanes.Add(laneIdx);
                }
            }
            
            float posZ = (float)(z * noteSpeed + zOffset);
            
            if (lanes.Count == 2) // 2個の場合は補助面ではなく線を生成
            {
                // 補助線、補助面は常に0番レーンのコンテナによって制御する
                var line = supportContainer.InstantiatePrefab(supportLinePrefab);
                
                var vm  = line.GetComponent<GameObjectContext>().Container.Resolve<ISupportLineViewModel>();
                vm.SetInitialPosZ(posZ);
                vm.SetGuids(guids);
                
                
                var lineRenderer = line.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, new Vector3((float)_laneXPositionCache[lanes[0]], (lanes[0] < 4) ? bottonYPosition + 0.001f : topYPosition - 0.001f, posZ));
                lineRenderer.SetPosition(1, new Vector3((float)_laneXPositionCache[lanes[1]], (lanes[1] < 4) ? bottonYPosition + 0.001f : topYPosition - 0.001f, posZ));
                lineRenderer.startWidth = 0.02f;
                lineRenderer.endWidth = 0.02f;
            }
            else if (lanes.Count >= 3)
            {
                var vertices = new List<Vector3>(lanes.Count);
                for (var i = 0; i < lanes.Count; i++)
                {
                    var lane = lanes[i];
                    // SupportPlane は transform の z 移動で制御するため、頂点はローカル座標で保持する
                    vertices.Add(new Vector3((float)_laneXPositionCache[lane], (lane < 4) ? bottonYPosition + 0.001f : topYPosition - 0.001f, 0f));
                }

                var mesh = GenerateMesh(vertices);
                // var supportPlane = new GameObject("SupportPlane", typeof(MeshFilter), typeof(MeshRenderer));
                var supportPlane = supportContainer.InstantiatePrefab(supportPlanePrefab);
                var vm  = supportPlane.GetComponent<GameObjectContext>().Container.Resolve<ISupportLineViewModel>();
                vm.SetInitialPosZ(posZ);
                vm.SetGuids(guids);
                supportPlane.GetComponent<MeshFilter>().mesh = mesh;
                supportPlane.transform.position = new Vector3(0f, 0f, posZ);
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

        private void SetNoteColor(MeshRenderer meshRenderer, NoteColor color)
        {
            var mat = meshRenderer.material;
            switch (color)
            {
                case NoteColor.Blue:
                    mat.EnableKeyword("_COLOR_BLUE");
                    mat.DisableKeyword("_COLOR_RED");
                    mat.DisableKeyword("_COLOR_EX");
                    break;
                case NoteColor.Red:
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
