using System;

namespace DefaultNamespace
{
    [Serializable]
    public class ChartData
    {
        public string chartPath;
        public float chartLevel; // 現状は自然数のみを用いるが、今後小数点以下の難易度を導入する可能性があるためfloatとしている
    }
}
