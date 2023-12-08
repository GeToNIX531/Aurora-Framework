using Buffers;
using AI_Aurora_V1.Data.Vector;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AI_Aurora_V1.Modules.AI.Base
{
    public class Batch : Client
    {
        public Batch(params int[] Count) : base(Count)
        {

        }

        public class TaskCompute1
        {
            Layer[] layers;
            int LCount;
            public TaskCompute1(Layer[] Layers, int LCount)
            {
                this.layers = Layers;
                this.LCount = LCount;
            }

            Vector<double> Input;
            Vector<double> Output;
            public void Set(Vector<double> Input, Vector<double> Output)
            {
                this.Input = Input;
                this.Output = Output;
            }

            public Layer.LBack[] Result;
            public void Compute()
            {
                var input = Input;
                Vector<double> output;

                BPointer<Vector<double>> buffer = new BPointer<Vector<double>>(LCount + 1);
                buffer.Add(input);
                for (int k = 0; k < LCount - 1; k++)
                {
                    buffer.Last(out var bufferValue);

                    output = layers[k].Out(bufferValue);
                    buffer.Add(output);
                }

                List<Layer.LBack> datas = new List<Layer.LBack>();
                output = Output;
                var data = layers[LCount - 1].Backward(buffer.Take(), output);
                datas.Add(data);

                for (int k = 1; k < LCount; k++)
                {
                    int index = LCount - 1 - k;


                    data = layers[index].BackwardHidden(buffer.Take(), data);
                    datas.Add(data);
                }

                datas.Reverse();
                Result = datas.ToArray();
            }
        }

        private class TaskCompute2
        {
            public Layer.LBack Result;

            Layer.LBack[] Datas;
            public TaskCompute2(Layer.LBack[] Datas)
            {
                this.Datas = Datas;
            }

            public void Compute()
            {
                int count = Datas[0].de_dt.Count;
                var de_dt = new double[count];

                for (int k = 0; k < count; k++)
                {
                    de_dt[k] = Datas.Average(T => T.de_dt[k]);
                }

                count = Datas[0].de_dh.Count;
                var de_dh = new double[count];

                for (int k = 0; k < count; k++)
                {
                    de_dh[k] = Datas.Average(T => T.de_dh[k]);
                }

                Matrix<double> de_dW = Matrix<double>.Build.DenseOfMatrix(Datas[0].de_dW);
                count = Datas.Length;

                for (int k = 1; k < count; k++)
                {
                    de_dW += Datas[k].de_dW;
                }

                de_dW /= count;

                Result = new Layer.LBack(de_dt.ToVector(), de_dW, de_dh.ToVector());
            }
        }

        public void Learning(Vector<double>[] Inputs, Vector<double>[] Outputs)
        {
            List<Layer.LBack[]> bufferV2 = new List<Layer.LBack[]>();

            int count = Inputs.Length;
            TaskCompute1[] taskComputes = new TaskCompute1[count];
            Task[] threads = new Task[count];
            for (int i = 0; i < count; i++)
            {
                var input = Inputs[i];
                var output = Outputs[i];

                taskComputes[i] = new TaskCompute1(layers, LCount);
                taskComputes[i].Set(input, output);
                threads[i] = Task.Factory.StartNew(taskComputes[i].Compute);
            }

            Task.WaitAll(threads);
            for (int i = 0; i < count; i++)
                bufferV2.Add(taskComputes[i].Result);

            count = LCount;
            TaskCompute2[] taskComputes2 = new TaskCompute2[count];
            threads = new Task[count];


            var datas = bufferV2.ToArray();
            for (int i = 0; i < count; i++)
            {
                var data = datas.Select(T => T[i]).ToArray();
                taskComputes2[i] = new TaskCompute2(data);
                threads[i] = Task.Factory.StartNew(taskComputes2[i].Compute);
            }

            for (int i = 0; i < count; i++)
            {
                threads[i].Wait();
                layers[i].Update(taskComputes2[i].Result);
            }

        }
    }
}
