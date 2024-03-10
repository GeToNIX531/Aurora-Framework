using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.AI.BaseV2
{
    public class IOData
    {
        public double[] Input { get; private set; }
        public double[] Output { get; private set; }

        public IOData(double[] Input, double[] Output)
        {
            this.Input = Input;
            this.Output = Output;
        }
    }
}
