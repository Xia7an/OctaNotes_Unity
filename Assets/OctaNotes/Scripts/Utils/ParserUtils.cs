using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OctaNotes.Scripts.Utils
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
                var lineSb = new StringBuilder();

                bool inString = false;
                bool escaped = false;

                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];

                    if (escaped)
                    {
                        escaped = false;
                        lineSb.Append(c);
                        continue;
                    }

                    if (c == '\\')
                    {
                        escaped = true;
                        lineSb.Append(c);
                        continue;
                    }

                    if (c == '"')
                    {
                        inString = !inString;
                        lineSb.Append(c);
                        continue;
                    }

                    // 文字列外で // を検出 → その行の残りは無視
                    if (!inString && c == '/' && i + 1 < line.Length && line[i + 1] == '/')
                    {
                        break;
                    }

                    lineSb.Append(c);
                }

                // 行頭・行末の空白を削除し、改行は追加しない
                sb.Append(lineSb.ToString().Trim(' ', '\t'));
            }

            return sb.ToString();
        }
        
        private static readonly Regex TokenRegex =
            new Regex(@"[A-Za-z][A-Za-z0-9]*|[-+]?\d+(?:\.\d+)?", RegexOptions.Compiled);

        public static List<string> Tokenize(string input)
        {
            var tokens = new List<string>();

            foreach (Match m in TokenRegex.Matches(input))
            {
                tokens.Add(m.Value);
            }

            return tokens;
        }
    }
}
