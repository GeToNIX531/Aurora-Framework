using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buffers
{
    public class BPointer<T>
    {
        private T[] array;
        public int Size { get; private set; }

        public BPointer(int Size)
        {
            array = new T[Size];
            this.Size = Size;
            index = 0;
        }

        private int index;
        public int Point
        {
            get => index;

            private set
            {
                if (value >= Size)
                    index = value - Size;
                else if (value < 0)
                    index = value + Size;
                else index = value;
            }
        }

        public void Add(T Element)
        {
            array[Point] = Element;
            Point++;
        }

        public void Add(params T[] Elements)
        {
            foreach (var value in Elements)
                Add(value);
        }

        public T Take()
        {
            Point--;
            return array[Point];
        }

        public bool Last(int Index, out T Value)
        {
            Point -= Index;
            Value = array[Point];
            Point += Index;

            if (Value == null)
                return false;

            return true;
        }

        public bool Last(out T Value)
        {
            Point--;
            Value = array[Point];
            Point++;

            if (Value == null)
                return false;

            return true;
        }

        public T[] ToArray() => array;

        public override string ToString()
        {
            string result = string.Empty;
            int count = Size - 1;
            for (int i = 0; i < count; i++)
                result += array[i] + " ";

            result += array[count];
            return result;
        }

        public void Clear()
        {
            index = 0;
            array = new T[Size];
        }
    }
}
