using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Main.Buffers
{
    public class DoubleBuffer
    {
        private double[][] array;
        public int Size { get; private set; }

        public DoubleBuffer(int Size)
        {
            this.Size = Size;
            Clear();
        }

        private int index;
        public int Point
        {
            get => index;
            private set {
                if (value >= Size)
                    index = Size - 1;
                else if (value < 0)
                    index = 0;
                else index = value;
            }
        }

        public void Add(double[] Value)
        {
            array[Point] = Value;
            Point++;
        }

        public void Clear()
        {
            array = new double[Size][];
            index = 0;
        }

        public double[][] Values => array;

        public bool Last(out double[] Value)
        {
            Point--;
            Value = array[Point];
            Point++;

            if (Value == null)
                return false;

            return true;
        }

        public bool Last(int Index, out double[] Value)
        {
            Point -= Index;
            Value = array[Point];
            Point += Index;

            if (Value == null)
                return false;

            return true;
        }

        public override string ToString()
        {
            string result = string.Empty;
            int count = Size - 1;
            for (int i = 0; i < count; i++)
                result += array[i] + " ";

            result += array[count];
            return result;
        }

        public bool IsFull() => index == Size - 1;
    }
}
