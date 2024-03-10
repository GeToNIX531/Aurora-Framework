using Aurora_Framework.Modules.AI.Base.BaseV2._1;
using Buffers;
using Function;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.AI.Base.BaseV2._1
{
    public class Client
    {
        public Layer[] layers;
        public int[] NCount;

        public int LCount;
        public int ICount;
        public int OCount;

        private readonly Random rnd;
        public Client(params int[] NCount)
        {
            rnd = new Random();
            this.NCount = NCount;
            LCount = NCount.Length - 1;
            ICount = NCount[0];
            OCount = NCount[LCount];

            layers = new Layer[LCount];
            int iCount = ICount;
            for (int l = 0; l < LCount - 1; l++)
            {
                int oCount = NCount[l + 1];
                layers[l] = new Layer(iCount, oCount, new Relu(), rnd);
                iCount = oCount;
            }

            layers[LCount - 1] = new Layer(iCount, OCount, new SoftMax(), rnd);
        }

        public void Learn(double[] Input, double[] Output, double Alpha = 0.1d)
        {
            BPointer<double[]> buffer = new BPointer<double[]>(LCount + 1);
            buffer.Add(Input);

            for (int i = 0; i < LCount - 1; i++)
            {
                buffer.Last(out var value);
                var output = layers[i].FunctionActivaion(value);
                buffer.Add(output);
            }

            LearnData data = null;
            for (int i = 0; i < LCount; i++)
            {
                int index = LCount - 1 - i;
                data = layers[index].GetLearnData(buffer.Take(), Output, data);
                layers[index].Update(data.de_dw, data.de_db, Alpha);
            }
        }

        public double[] Result(double[] Input)
        {
            double[] input = Input;
            for (int l = 0; l < LCount; l++)
                input = layers[l].FunctionActivaion(input);
            return input;
        }

        public double[] Loss(double[] Input, double[] Output)
        {
            var result = Result(Input);
            double[] loss = new double[OCount];

            for (int k = 0; k < OCount; k++)
                loss[k] = result[k] - Output[k];

            return loss;
        }

        public double LossV2(double[] Input, double[] Output)
        {
            double[] loss = Loss(Input, Output);
            for (int i = 0; i < loss.Length; i++)
                loss[i] = loss[i] * loss[i];

            
            return loss.Sum();
        }
    }

    public class Layer
    {
        public double[,] W;
        public double[] B;

        public readonly int ICount;
        public readonly int OCount;

        private IFunctionActivation Function;

        public Layer(int Input, int Output, IFunctionActivation Function, Random RND)
        {
            W = new double[Input, Output];
            B = new double[Output];
            this.Function = Function;

            ICount = Input;
            OCount = Output;

            for (int k = 0; k < OCount; k++)
            {
                for (int i = 0; i < ICount; i++)
                    W[i, k] = RND.NextDouble();

                B[k] = RND.NextDouble();
            }
        }

        public double[] Result(double[] Input)
        {
            double[] result = new double[OCount];
            lock (W)
                lock (B)
                {
                    for (int k = 0; k < OCount; k++)
                    {
                        double value = default;
                        for (int i = 0; i < ICount; i++)
                            value += Input[i] * W[i, k];

                        value += B[k];
                        result[k] = value;
                    }
                }
            return result;
        }

        public double[] FunctionActivaion(double[] Input)
        {
            double[] result = new double[OCount];
            for (int k = 0; k < OCount; k++)
            {
                double value = default;
                for (int i = 0; i < ICount; i++)
                    value += Input[i] * W[i, k];

                result[k] = value + B[k];
            }

            return Function.Use(result);
        }

        public double[] Loss(double[] AIResult, double[] Output)
        {
            for (int k = 0; k < OCount; k++)
                AIResult[k] = AIResult[k] - Output[k];

            return AIResult;
        }

        public void Update(double[,] WCoueficent, double[] BCoeficent, double Alpha)
        {
            lock (W)
                lock (B)
                {
                    for (int k = 0; k < OCount; k++)
                    {
                        for (int i = 0; i < ICount; i++)
                            W[i, k] -= WCoueficent[i, k] * Alpha;

                        B[k] -= BCoeficent[k] * Alpha;
                    }
                }
        }

        public LearnData GetLearnData(double[] Input, double[] Output, LearnData LearnData = null)
        {
            double[] result = FunctionActivaion(Input);
            double[] de_dt;

            if (LearnData == null) de_dt = Loss(result, Output);
            else
            {
                var derive = Function.Derive(result);
                de_dt = new double[OCount];

                for (int k = 0; k < OCount; k++)
                {
                    de_dt[k] = LearnData.de_dh[k] * derive[k];
                }
            }

            double[,] de_dw = new double[ICount, OCount];
            for (int i = 0; i < ICount; i++)
            {
                for (int k = 0; k < OCount; k++)
                {
                    de_dw[i, k] = de_dt[k] * Input[i];
                }
            }

            double[] de_dh = new double[ICount];
            for (int i = 0; i < ICount; i++)
            {
                double value = default;

                for (int k = 0; k < OCount; k++)
                {

                    value += de_dt[k] * W[i, k];
                }

                de_dh[i] = value;
            }

            return new LearnData(de_dh, de_dt, de_dw);
        }
    }

    public interface IFunctionActivation
    {
        double[] Use(double[] Value);
        double[] Derive(double[] Value);
    }

    public class LearnData
    {
        public double[] de_dh;
        public double[] de_dt;

        public double[,] de_dw;
        public double[] de_db => de_dt;

        public LearnData(double[] de_dh, double[] de_dt, double[,] de_dw)
        {
            this.de_dh = de_dh;
            this.de_dt = de_dt;

            this.de_dw = de_dw;
        }

    }



}


namespace Function
{
    public class Relu : IFunctionActivation
    {
        public double[] Derive(double[] Value)
        {
            int count = Value.Length;
            for (int i = 0; i < count; i++)
                Value[i] = Value[i] > 0 ? 1 : 1;
            return Value;
        }

        public double[] Use(double[] Value)
        {
            int count = Value.Length;
            for (int i = 0; i < count; i++)
                Value[i] = Math.Max(0, Value[i]);
            return Value;
        }
    }

    public class SoftMax : IFunctionActivation
    {
        public double[] Derive(double[] Value)
        {
            throw new Exception("SoftMax не обладает производной!");
        }

        public double[] Use(double[] Value)
        {

            var z_exp = Value.Select(Math.Exp);
            // [2.72, 7.39, 20.09, 54.6, 2.72, 7.39, 20.09]

            var sum_z_exp = z_exp.Sum();
            // 114.98

            if(sum_z_exp == 0)
                throw new Exception("SoftMax ошибка: sum_z_exp == 0");

            var softmax = z_exp.Select(i => i / sum_z_exp);

            return softmax.ToArray();
        }

        public static double[] StaticUse(double[] Value)
        {

            var z_exp = Value.Select(Math.Exp);
            // [2.72, 7.39, 20.09, 54.6, 2.72, 7.39, 20.09]

            var sum_z_exp = z_exp.Sum();
            // 114.98

            if (sum_z_exp == 0)
                throw new Exception("SoftMax ошибка: sum_z_exp == 0");

            var softmax = z_exp.Select(i => i / sum_z_exp);

            return softmax.ToArray();
        }
    }
}