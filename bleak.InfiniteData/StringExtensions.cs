using System.Collections.Generic;
using System.Text;

namespace bleak.InfiniteData
{

    public static class StringExtensions
    {
        public static string[] QualifiedSplit(this string input, string separator, string textqualifier = null)
        {
            if (string.IsNullOrEmpty(textqualifier))
            {
                return input.Split(separator.ToCharArray());
            }

            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool firstchar = true;
            bool qualified = false;
            for (int i = 0; i < input.Length; i++)
            {
                var strc = input.Substring(i, 1);

                if (firstchar)
                {
                    firstchar = false;
                    if (strc == textqualifier)
                    {
                        qualified = true;
                        continue;
                    }
                    else
                    {
                        qualified = false;
                    }
                }

                if (qualified)
                {
                    if (strc == textqualifier)
                    {
                        if (input.Length < (i + textqualifier.Length + separator.Length))
                        {
                            i += separator.Length;
                            var token = sb.ToString();
                            tokens.Add(token);
                            sb.Clear();
                            firstchar = true;
                            continue;
                        }

                        var next = input.Substring(i + textqualifier.Length, separator.Length);
                        if (next == separator)
                        {
                            i += separator.Length;
                            var token = sb.ToString();
                            tokens.Add(token);
                            sb.Clear();
                            firstchar = true;
                            continue;
                        }
                        if (next == textqualifier)
                        {
                            i += textqualifier.Length;
                        }
                    }
                }
                else
                {   // not qualified
                    if (strc == separator)
                    {
                        var token = sb.ToString();
                        tokens.Add(token);
                        sb.Clear();
                        firstchar = true;
                        continue;
                    }
                }

                sb.Append(strc);
            }

            if (sb.ToString().Length > 0 || (firstchar && !qualified))
            {
                var token = sb.ToString();
                tokens.Add(token);
            }

            return tokens.ToArray();
        }
    }
}