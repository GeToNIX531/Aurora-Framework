using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buffers.Tokenezator
{
    public class Inputer
    {
        private Dictionary<string, long> tokens;
        public Inputer()
        {
            tokens = new Dictionary<string, long>();
        }

        public bool TryAdd(string Value)
        {
            if (tokens.ContainsKey(Value))
            {
                tokens[Value]++;
                return false;
            }

            tokens.Add(Value, 0);
            return true;
        }

        public void Add(params string[] Values)
        {
            foreach (var value in Values)
                TryAdd(value);
        }

        public bool Contains(string Key, out long Value) => tokens.TryGetValue(Key, out Value);

        public Outputer ToOutputer() => new Outputer(tokens);

        public int Count => tokens.Keys.Count;
    }
}
