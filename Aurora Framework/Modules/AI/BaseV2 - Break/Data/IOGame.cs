using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.AI.BaseV2
{
    public class GameIOData : IDisposable
    {
        public CData[] datas;
        public int Size { get; private set; }

        public GameIOData(int CacheSize = 128 * 20)
        {
            Size = CacheSize;
            datas = new CData[Size];
            for (int i = 0; i < Size; i++)
                datas[i] = new CData();
        }

        private int Index = -1;
        private double startScore = -1;
        private double currentScore = -1;

        public void Clear()
        {
            Index = -1;
        }

        public void Add(double[] input, double[] output, double score)
        {
            if (Index == Size - 1) Clear();
            if (Index == -1) startScore = score;
            currentScore = score;
            Index++;
            datas[Index].Update(input, output, score);
        }

        public CData[] Get()
        {
            var result = new CData[Index];
            datas.CopyTo(result, 0);
            return result;
        }

        public bool isFull() => Index == Size - 1;
        public bool isReady()
        {
            if (Index == Size - 1) return false;
            if (Index == -1) return false;
            if (startScore == currentScore) return false;
            return true;
        }

        public void Dispose()
        {
            for (int i = 0; i < Size; i++)
            {
                datas[i].Dispose();
                datas[i] = null;
            }

            datas = null;
            Index = -1;
            startScore = default;
            currentScore = default;
        }

        public class CData : IDisposable
        {
            public double[] Input { get; private set; }
            public double[] Output { get; private set; }
            public double Score { get; private set; }

            public CData(double[] input, double[] output, double score)
            {
                this.Input = input;
                this.Output = output;
                this.Score = score;
            }

            public CData() { }

            public void Update(double[] input, double[] output, double score)
            {
                this.Input = input;
                this.Output = output;
                this.Score = score;
            }

            public void Dispose()
            {
                Input = null;
                Output = null;
                Score = default;
            }
        }
    }

}
