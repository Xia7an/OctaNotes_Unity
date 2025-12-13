using System.IO;
using System.Text;

namespace OctaNotes.Scripts.ParserUtils
{
    public class ParserUtils
    {
        public static string RemoveLineCommentsSmart(string code)
        {
            var reader = new StringReader(code);
            var sb = new StringBuilder();

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                bool inString = false;
                bool escaped = false;

                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];

                    if (escaped)
                    {
                        // 直前が \ の場合はエスケープされた文字
                        escaped = false;
                        sb.Append(c);
                        continue;
                    }

                    if (c == '\\')
                    {
                        escaped = true;
                        sb.Append(c);
                        continue;
                    }

                    if (c == '"')
                    {
                        inString = !inString;
                        sb.Append(c);
                        continue;
                    }

                    // 文字列外で // を検出 → その行の残りは無視
                    if (!inString && c == '/' && i + 1 < line.Length && line[i + 1] == '/')
                    {
                        break;
                    }

                    sb.Append(c);
                }

                sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}
