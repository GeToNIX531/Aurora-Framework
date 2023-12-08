using AI_Aurora_V1.Data.Vector;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Aurora_V1.Modules.AI.Base.TrainData
{
    public static class Find50
    {
        private static Vector<double> Get(int Number)
        {
            return new double[]
            {
                Number > 50 ? 1d : 0d,
                Number < 50 ? 1d : 0d,
                Number == 50 ? 1d : 0d,
            }.ToVector();
        }

        static int minValue = 1;
        static int currentValue = minValue;
        static int maxValue = 101;
        public static Data Get()
        {
            Vector<double> input = Vector<double>.Build.Dense(new double[] { currentValue });
            Vector<double> output = Get(currentValue);

            currentValue++;
            if (currentValue > maxValue)
                currentValue = minValue;

            return new Data(input, output);
        }


        public static Data[] BigData()
        {
            int start = 45;
            int end = 55;

            int count = end - start;
            Data[] result = new Data[count];

            for (int i = 0; i < count; i++)
            {
                Vector<double> input = Vector<double>.Build.Dense(new double[] { start + i });
                Vector<double> output = Get(start + i);
                result[i] = new Data(input, output);
            }

            return result;
        }

    }

    public class Data
    {
        public Vector<double> Input;
        public Vector<double> Outputs;

        public Data(Vector<double> Input, Vector<double> Outputs)
        {
            this.Input = Input;
            this.Outputs = Outputs;
        }
    }
}
