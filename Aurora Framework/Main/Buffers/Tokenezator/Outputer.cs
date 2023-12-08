using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buffers.Tokenezator
{
    public class Outputer
    {
        private Dictionary<int, string> IndexTokens;
        private Dictionary<string, int> TextTokens;

        public Outputer(Dictionary<string, long> tokens)
        {
            var temp = tokens.OrderByDescending(T => T.Value).Select(T => T.Key).ToArray();

            IndexTokens = new Dictionary<int, string>();
            TextTokens = new Dictionary<string, int>();

            int count = temp.Length;
            for (int i = 0; i < count; i++)
            {
                IndexTokens.Add(i, temp[i]);
                TextTokens.Add(temp[i], i);
            }
        }

        public bool FromText(string Value, out int Index) => TextTokens.TryGetValue(Value, out Index);
        public bool FromIndex(int Index, out string Value) => IndexTokens.TryGetValue(Index, out Value);

        public bool FromText(out int[] Indexes, params string[] Text)
        {
            List<int> indexes = new List<int>();

            bool result = true;
            foreach (var value in Text)
                if (FromText(value, out int Index) == false)
                {
                    result = false;
                    break;
                }
                else indexes.Add(Index);

            Indexes = indexes.ToArray();
            return result;
        }

        public bool FromIndexes(out string[] Values, params int[] Indexes)
        {
            List<string> values = new List<string>();
            bool result = true;

            foreach (var value in Indexes)
                if (FromIndex(value, out string Value) == false)
                {
                    result = false;
                    break;
                }
                else values.Add(Value);

            Values = values.ToArray();
            return result;
        }

        public bool FromTextV2(string Text, out int[] Indexes, out string NotFind)
        {
            List<int> indexes = new List<int>();
            NotFind = string.Empty;

            int start = 0;
            int count = Text.Length;
            bool result = true;
            do
            {
                string Token;
                if (start + 2 <= count)
                {
                    Token = Text.Substring(start, 2);
                    if (TextTokens.ContainsKey(Token))
                    {
                        indexes.Add(TextTokens[Token]);
                        start = start + 2;
                        continue;
                    }
                }

                Token = Text.Substring(start, 1);
                if (TextTokens.ContainsKey(Token))
                {
                    indexes.Add(TextTokens[Token]);
                    start = start + 1;
                }
                else
                {
                    result = false;
                    NotFind = Token;
                    break;
                }
            }
            while (start != count);

            Indexes = indexes.ToArray();
            return result;
        }
    }
}
