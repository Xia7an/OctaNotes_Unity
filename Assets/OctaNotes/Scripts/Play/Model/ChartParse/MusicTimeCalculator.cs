namespace OctaNotes.Scripts.Play.Model
{
    using System;
using System.Collections.Generic;

/// <summary>
/// 音楽位置・時刻・ノーツ座標を一貫して計算するクラス
/// BPM / 拍子 / HS の途中変更に対応
/// </summary>
public class MusicTimeCalculator
{
    #region internal event definitions

    private class BPMChange
    {
        public int bar;
        public int m;
        public int n;
        public double bpm;
    }

    private class BeatChange
    {
        public int bar;
        public int M;
        public int N;
    }

    private class HSChange
    {
        public int bar;
        public int m;
        public int n;
        public double hs;
    }

    #endregion

    #region fields

    private readonly List<BPMChange> bpmChanges = new();
    private readonly List<BeatChange> beatChanges = new();
    private readonly List<HSChange> hsChanges = new();

    private double initialBPM;
    private int initialM;
    private int initialN;
    private double initialHS;

    #endregion

    #region constructor

    public MusicTimeCalculator(
        double bpm,
        int M,
        int N,
        double hs = 1.0)
    {
        initialBPM = bpm;
        initialM = M;
        initialN = N;
        initialHS = hs;
    }

    #endregion

    #region public API

    /// <summary>
    /// 音楽位置 (l 小節, m 個目の n 分音符) の時刻 [秒]
    /// </summary>
    public double CalcTime(int l, int m, int n)
    {
        double time = 0.0;

        double currentBPM = initialBPM;
        int currentM = initialM;
        int currentN = initialN;

        int bar = 0;
        int bpmIdx = 0;
        int beatIdx = 0;

        // 小節単位で積分
        while (bar < l)
        {
            // 拍子変更
            if (beatIdx < beatChanges.Count &&
                beatChanges[beatIdx].bar == bar)
            {
                currentM = beatChanges[beatIdx].M;
                currentN = beatChanges[beatIdx].N;
                beatIdx++;
            }

            double beatsInBar = currentM * (4.0 / currentN);
            double consumed = 0.0;

            // 小節途中の BPM 変更
            while (bpmIdx < bpmChanges.Count &&
                   bpmChanges[bpmIdx].bar == bar)
            {
                var bc = bpmChanges[bpmIdx];
                double beatPos = bc.m * (4.0 / bc.n);

                double delta = beatPos - consumed;
                time += delta * (60.0 / currentBPM);

                consumed = beatPos;
                currentBPM = bc.bpm;
                bpmIdx++;
            }

            time += (beatsInBar - consumed) * (60.0 / currentBPM);
            bar++;
        }

        // 最終小節内
        double targetBeats = m * (4.0 / n);
        double progressed = 0.0;

        while (bpmIdx < bpmChanges.Count &&
               bpmChanges[bpmIdx].bar == l)
        {
            var bc = bpmChanges[bpmIdx];
            double beatPos = bc.m * (4.0 / bc.n);

            if (beatPos >= targetBeats)
                break;

            double delta = beatPos - progressed;
            time += delta * (60.0 / currentBPM);

            progressed = beatPos;
            currentBPM = bc.bpm;
            bpmIdx++;
        }

        time += (targetBeats - progressed) * (60.0 / currentBPM);

        return time;
    }

    /// <summary>
    /// 指定時刻におけるノーツ座標
    /// </summary>
    public double CalcPosition(double time)
    {
        double pos = 0.0;

        double currentHS = initialHS;
        double currentTime = 0.0;

        int hsIdx = 0;

        while (hsIdx < hsChanges.Count)
        {
            var hc = hsChanges[hsIdx];
            double changeTime = CalcTime(hc.bar, hc.m, hc.n);

            if (changeTime > time)
                break;

            double dt = changeTime - currentTime;
            pos += currentHS * dt;

            currentTime = changeTime;
            currentHS = hc.hs;
            hsIdx++;
        }

        pos += currentHS * (time - currentTime);
        return pos;
    }

    /// <summary>
    /// 小節 l から拍子を変更
    /// </summary>
    public void ChangeBeat(int l, int newM, int newN)
    {
        beatChanges.Add(new BeatChange
        {
            bar = l,
            M = newM,
            N = newN
        });

        beatChanges.Sort((a, b) => a.bar.CompareTo(b.bar));
    }

    /// <summary>
    /// 小節 l の m 個目の n 分音符から BPM を変更
    /// </summary>
    public void ChangeBPM(int l, int m, int n, double bpm)
    {
        bpmChanges.Add(new BPMChange
        {
            bar = l,
            m = m,
            n = n,
            bpm = bpm
        });

        SortBPMChanges();
    }

    /// <summary>
    /// 指定音楽位置から HS を変更
    /// </summary>
    public void ChangeHS(double hs, int l, int m, int n)
    {
        hsChanges.Add(new HSChange
        {
            bar = l,
            m = m,
            n = n,
            hs = hs
        });

        SortHSChanges();
    }

    #endregion

    #region helpers

    private void SortBPMChanges()
    {
        bpmChanges.Sort((a, b) =>
        {
            int c = a.bar.CompareTo(b.bar);
            if (c != 0) return c;

            double pa = a.m * (1.0 / a.n);
            double pb = b.m * (1.0 / b.n);
            return pa.CompareTo(pb);
        });
    }

    private void SortHSChanges()
    {
        hsChanges.Sort((a, b) =>
        {
            int c = a.bar.CompareTo(b.bar);
            if (c != 0) return c;

            double pa = a.m * (1.0 / a.n);
            double pb = b.m * (1.0 / b.n);
            return pa.CompareTo(pb);
        });
    }

    #endregion
}

}
