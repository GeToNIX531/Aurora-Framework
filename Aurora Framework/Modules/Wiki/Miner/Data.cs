using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hash;

namespace Aurora_Framework.Modules.Wiki.V1._0.Miner
{
    public class Data
    {
        public string Start;
        public long Step;
        public int Index;

        public string Text;
        public string Name;

        public string Hash;

        public Data(string start, long step, int index, string text, string name)
        {
            Start = start;
            Step = step;
            Index = index;
            Text = text;
            Name = name;



            Hash = text.TTHashGet(25);
        }
    }
}
