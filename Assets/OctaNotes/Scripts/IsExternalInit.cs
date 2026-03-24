// Unity の Mono ランタイムは C# 9 の init setter に必要な
// System.Runtime.CompilerServices.IsExternalInit を含まないため、ここで定義します。
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
