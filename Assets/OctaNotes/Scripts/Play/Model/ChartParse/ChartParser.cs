using System;
using System.Collections.Generic;
using System.Linq;
using OctaNotes.Scripts.Core.Model;
using OctaNotes.Scripts.Play.Interface;
using UnityEngine;
using Zenject;


// Todo: Typedef的のを書く
// global using GraphicalChart Dictionary<double, List<string>>;
namespace OctaNotes.Scripts.Play.Model
{
    public class ChartParser: IChartParser, IInitializable
    {
        public Dictionary<double, List<GraphicalNoteEntry>> GraphicalChartData => _chartData1;
        public List<List<NoteTiming>> LaneWiseChartData => _chartData2;
        public List<(double,double)> HsChangeData => _hschangeData;


        private Dictionary<double, List<GraphicalNoteEntry>> _chartData1 = new(); // ノーツのオブジェクトを画面上に描画するための譜面データ構造
        // Keyはノーツを生成するz座標、Valueには8つのレーンと32個の誘導線の系列のデータが格納されている。
        // Valueの例: ["b2","b1","r1","r2","","","",""," ]
        // ソフランは考えないでいい！(because そもそも位置が与えられており、速度変化はイベント発火で処理するため)
        
        // レーン毎のノーツを管理する譜面データ構造
        // indexがレーン番号、Valueがそのレーンに存在するノーツのタイミング一覧
        private List<List<NoteTiming>> _chartData2 = new List<List<NoteTiming>>(8);
        
        private List<(double,double)> _hschangeData = new List<(double,double)>(); // (時刻, hs倍率)

        private int M, N; // M,M :N分のM拍子
        private double bpm;
        private int l, m, n; // 今解釈中の行がどのタイミングなのかを保持する変数。 l小節目のm番目のn分音符を解釈中
        private double _timing = 0f;
        private double _noteZPosition = 0f;
        private int currentline = 0;
        private string _version;
        
        private bool _initializing = false;
        private bool _initialized = false;
        private bool _isvalid = false;
        
        private MusicTimeCalculator _calculator;

        private HashSet<string> validNoteTypeStr = new HashSet<string>()
        {
            "b1", "b2", "b3", "b4",
            "bc1", "bc2", "bc3", "bc4",
            "lb1", "lb2", "lb3", "lb4", 
            "r1", "r2", "r3", "r4",
            "rc1", "rc2", "rc3", "rc4",
            "lr1", "lr2", "lr3", "lr4",
            "le",
            "w", "lw", "cw"
        };

        private List<string> ChartData;
        
        public void Initialize()
        {
            LoadChart(Application.persistentDataPath + "/Charts/ShiningStar/octa.onc");
        }
        
        public void LoadChart(string path)
        {
            // 譜面データは各行を文字列とするリストで受け取る
            var data = System.IO.File.ReadAllLines(path);
            ChartData = new List<string>(data); //.oncファイルの各行をプレーンテキストで要素に持つリスト
            _chartData1 = new Dictionary<double, List<GraphicalNoteEntry>>();
            _chartData2 = new List<List<NoteTiming>>(8);
            for (int i = 0; i < 8; i++)
            {
                _chartData2.Add(new List<NoteTiming>());
            }
            Parse();
        }
        
        private void Parse()
        {
            foreach (var rawline in ChartData)
            {
                currentline++;
                var line = ParserUtils.ParserUtils.RemoveLineCommentsSmart(rawline);
                if(line.Length <= 1) continue; // 空行(コメント行)はスキップ

                if (line[0] == '#')
                {
                    switch (line.Split().First())
                    {
                        case "VERSION":
                            _version = line.Split().Last();
                            break;
                        case "OCTANOTES_CHART":
                            _isvalid = true;
                            break;
                    }
                    continue;
                }

                // if (!_isvalid)
                // {
                //     throw new InvalidPrefixException();
                // }
                if (line.Last() == ':') // ラベルだったらこっち
                {
                    if (line.ToLower()[..4] != "init") // 通常のタイミング指定の場合
                    {
                        if (!_initialized)
                        {
                            ShowErrorMessage("initセクションがありませんでした。");
                        }
                        _initializing = false;
                        var lmn = line[..^1].Split(",").Select(int.Parse).ToList();
                        l = lmn[0]; m = lmn[1]; n = lmn[2];
                    }
                    else 
                    {
                        _initializing = true;
                    }
                }
                else // 通常の命令だったらこっち
                {
                    var tokens = ParserUtils.ParserUtils.Tokenize(line);
                    string opcode = tokens[0];
                    string[] operands = tokens.Skip(1).ToArray();
                    switch (opcode.ToLower())
                    {
                        case "bpm":
                            SetBpm(operands);
                            break;
                        case "beat":
                            SetBeats(operands);
                            break;
                        case "n":
                            SetNotes(operands);
                            break;
                        case "l":
                            SetLine(operands);
                            break;
                        case "hs":
                            SetHS(operands);
                            break;
                    }
                }

                // 初期化セクション内で、BPMと拍子が設定されたら、caluclatorを初期化する
                if (_initializing)
                {
                    if (bpm != 0f && M != 0 && N != 0)
                    {
                        _calculator = new MusicTimeCalculator(bpm, M, N);
                        _initialized = true;
                        _initializing = false;
                    }
                }
            }
        }
        
        private void SetBpm(string[] operands)
        {
            try
            {
                bpm = double.Parse(operands[0]);
                if(_initialized) _calculator.ChangeBPM(l, m, n, bpm);
            }
            catch (System.Exception)
            {
                ShowErrorMessage("Invalid BPM number");
            }
        }
        private void SetBeats(string[] operands)
        {
            try
            {
                M = int.Parse(operands[0]);
                N = int.Parse(operands[1]);
                if(_initialized) _calculator.ChangeBeat(l, M, N);
            }
            catch (System.Exception)
            {
                ShowErrorMessage("Invalid Beats number");
            }
        }
        private void SetNotes(string[] operands)
        {
            try
            {
                Guid noteGuid = Guid.NewGuid();
                var noteTypeStr = operands[0];
                int laneNum = int.Parse(operands[1]);
                if (!validNoteTypeStr.Contains(noteTypeStr))
                {
                    ShowErrorMessage("ノーツタイプが不正です");
                    return;
                }

                var timing = _calculator.CalcTime(l, m, n);
                var noteZPos = _calculator.CalcPosition(timing);
                
                NoteType noteType;
                char colorChar = '\0';
                if ((noteTypeStr[0] == 'b' || noteTypeStr[0] == 'r' || noteTypeStr[0] == 'w') && noteTypeStr.Length <= 2)
                {
                    noteType = NoteType.Tap;
                    colorChar = noteTypeStr[0];
                }
                else if (noteTypeStr[1] == 'c')
                {
                    noteType = NoteType.Chain;
                    colorChar = noteTypeStr[0];
                }
                else if (noteTypeStr == "le")
                {
                    noteType = NoteType.LongEnd;
                }
                else if (noteTypeStr[0] == 'l')
                {
                    noteType = NoteType.LongStart;
                    colorChar = noteTypeStr[1];
                }
                else
                {
                    ShowErrorMessage("ノーツタイプが不正です");
                    return;
                }
                
                NoteColor noteColor = colorChar switch
                {
                    'r' => NoteColor.Red,
                    'w' => NoteColor.Yellow,
                    _  => NoteColor.Blue
                };

                if(!int.TryParse(noteTypeStr.Last().ToString(), out int noteColorLevel)) noteColorLevel = 0;
                
                // そのタイミングに他のノーツがあれば、行の要素を書き換え
                if (_chartData1.TryGetValue(noteZPos, out var value))
                {
                    value[laneNum] = new GraphicalNoteEntry()
                    {
                        guid = noteGuid,
                        noteType = noteType,
                        noteColor = noteColor,
                        noteColorLevel = noteColorLevel,
                    };
                }
                // そのタイミングに他のノーツがない場合は、そのタイミングをKeyとして辞書に要素を追加
                else
                {
                    _chartData1[noteZPos] = new List<GraphicalNoteEntry>(new GraphicalNoteEntry[40])
                    {
                        [laneNum] = new  GraphicalNoteEntry()
                        {
                            guid = noteGuid,
                            noteType =  noteType,
                            noteColor = noteColor,
                            noteColorLevel = noteColorLevel,
                        }
                    };
                }


                _chartData2[laneNum].Add(new NoteTiming()
                {
                    timing = timing,
                    noteType = noteType,
                    guid = noteGuid,
                    isEx = noteColor == NoteColor.Yellow // 黄色ノーツはExとして扱う
                });
            }
            catch (System.InvalidOperationException)
            {
                ShowErrorMessage("オペランドが不正です。");
            }
        }
        private void SetLine(string[] operands)
        {
            try
            {
                if (operands[0][1] != 'x') throw new ArgumentException(); // オペランドの記法違反
                var lineSeries = int.Parse(operands[0][1..]);
                var pointTypeStr = operands[1];
                var timing = _calculator.CalcTime(l, m, n);
                var noteZPos = _calculator.CalcPosition(timing);
                if (_chartData1.TryGetValue(noteZPos, out var value))
                {
                    // 誘導線系列はリストの8番目から始まる
                    value[lineSeries + 8] = new GraphicalNoteEntry()
                    {
                        guid = Guid.Empty,
                        noteType = pointTypeStr[0] switch
                        {
                            's' => NoteType.LineStart,
                            'j' => NoteType.LineMid,
                            'e' => NoteType.LineEnd
                        },
                        noteColor = pointTypeStr[1] switch
                        {
                            'b' => NoteColor.Blue,
                            'r' => NoteColor.Red,
                            'w' => NoteColor.Yellow
                        },
                        noteColorLevel = pointTypeStr.Length >= 3 ? int.Parse(pointTypeStr[2].ToString()) : 0,
                    };
                }
                else
                {
                    _chartData1[noteZPos] = new List<GraphicalNoteEntry>(new GraphicalNoteEntry[40])
                    {
                        [lineSeries + 8] = new GraphicalNoteEntry()
                        {
                            guid = Guid.Empty,
                            noteType = pointTypeStr[0] switch
                            {
                                's' => NoteType.LineStart,
                                'j' => NoteType.LineMid,
                                'e' => NoteType.LineEnd
                            },
                            noteColor = pointTypeStr[1] switch
                            {
                                'b' => NoteColor.Blue,
                                'r' => NoteColor.Red,
                                'w' => NoteColor.Yellow
                            },
                            noteColorLevel = pointTypeStr.Length >= 3 ? int.Parse(pointTypeStr[2].ToString()) : 0,
                        }
                    };
                }
            }
            catch (Exception e)
            {
                ShowErrorMessage("オペランドが不正です。");
            }
        }
        private void SetHS(string[] operands)
        {
            try
            {
                var hsRate = double.Parse(operands[0]);
                var timing = _calculator.CalcTime(l, m, n);
                _calculator.ChangeHS(hsRate, l, m, n);
                _hschangeData.Add((timing, hsRate));
            }
            catch (Exception e)
            {
                ShowErrorMessage("オペランドが不正です。");
            }
        }
        
        private void ShowErrorMessage(string message)
        {
            Debug.LogError($"ChartParser line {currentline}: {message}");
        }
    }
}
