using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using Buffers;

namespace Aurora_Framework.Modules.AI.BaseV2
{
    public class Client
    {
        public Layer[] layers;
        public int[] NCount;

        public int LCount { get; private set; }
        public int ICount { get; private set; }
        public int OCount { get; private set; }

        private readonly Random rnd;
        public Client(params int[] NCount)
        {
            rnd = new Random();
            this.NCount = NCount;
            LCount = NCount.Length - 1;
            ICount = NCount[0];
            OCount = NCount[LCount];
            //OCount = NCount[LCount - 1];

            layers = new Layer[LCount];

            int iCount = ICount;
            for (int l = 0; l < LCount; l++)
            {
                int oCount = NCount[l + 1];
                layers[l] = new Layer(iCount, oCount, rnd);
                iCount = oCount;
            }

        }

        public double[] Result(double[] Input)
        {
            double[] input = Input;
            for (int l = 0; l < LCount; l++)
            {
                input = layers[l].Result(input);
            }

            return input;
        }

        public void Learn(double[] Input, double[] Output, double Alpha = 0.01d)
        {

            BPointer<double[]> buffer = new BPointer<double[]>(LCount + 1);
            buffer.Add(Input);

            for (int i = 0; i < LCount - 1; i++)
            {
                buffer.Last(out var value);
                var output = layers[i].Result(value);
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

        public double Loss(double[] Input, double[] Output)
        {
            var result = Result(Input);
            double loss = 0;

            for (int k = 0; k < OCount; k++)
            {
                loss += Output[k] - result[k];
            }

            return loss / OCount;
        }
    }

    public class Layer
    {
        public double[,] W;
        public double[] B;

        public int ICount { get; private set; }
        public int OCount { get; private set; }

        public Layer(int Input, int Output, Random RND)
        {
            W = new double[Input, Output];
            B = new double[Output];

            ICount = Input;
            OCount = Output;

            for (int k = 0; k < OCount; k++)
            {
                for (int i = 0; i < ICount; i++)
                {
                    W[i, k] = RND.NextDouble();
                }

                B[k] = (RND.NextDouble() - 0.5d) * 2d;
            }
        }

        public double[] Result(double[] Input)
        {
            double[] result = new double[OCount];
            for (int k = 0; k < OCount; k++)
            {
                double value = 0;

                for (int i = 0; i < ICount; i++)
                {
                    value += Input[i] * W[i, k];
                }

                value += B[k];
                result[k] = FunctionActivationReLu(value);
            }

            return result;
        }

        public double[] NError(double[] aiResult, double[] Output)
        {
            for (int k = 0; k < OCount; k++)
            {
                aiResult[k] = Output[k] - aiResult[k];
            }

            return aiResult;
        }


        private double coefError => 1 / OCount;
        public double LError(double[] NError)
        {
            double result = 0;

            foreach (double value in NError)
                result += value;

            return result * coefError;
        }

        public void Update(double[,] WCoueficent, double[] BCoeficent, double Alpha)
        {
            for (int k = 0; k < OCount; k++)
            {
                for (int i = 0; i < ICount; i++)
                {
                    W[i, k] += WCoueficent[i, k] * Alpha;
                }

                B[k] += BCoeficent[k] * Alpha;
            }
        }

        public double FunctionActivationReLu(double Value) => Math.Max(0, Value);
        public double DeriveFunctionActivaionReLu(double Value) => Value > 0 ? 1 : 0;

        public LearnData GetLearnData(double[] Input, double[] Output, LearnData LearnData = null)
        {
            double[] result = Result(Input);
            

            double[] de_dt = new double[OCount];
            if (LearnData == null)
            {
                double[] errors = NError(result, Output);
                for (int k = 0; k < OCount; k++)
                {
                    de_dt[k] = errors[k];
                }
            }
            else
            {
                for (int k = 0; k < OCount; k++)
                {
                    de_dt[k] = LearnData.de_dh[k] * DeriveFunctionActivaionReLu(result[k]);
                }
            }

            double[,] de_dw = new double[ICount, OCount];
            double[] de_db = new double[OCount];

            de_db = de_dt;

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


            return new LearnData()
            {
                de_dt = de_dt,
                de_db = de_db,
                de_dw = de_dw,
                de_dh = de_dh
            };














                /*
                double[] de_dx = new double[ICount];
                for (int i = 0; i < ICount; i++)
                {
                    double value = default;
                    for (int k = 0; k < OCount; k++)
                    {
                        value += de_dt[k] * W[i, k];
                    }

                    de_dx[i] = value;
                }
                */
        }
    }

    public class TLayer : Layer
    {
        public TLayer(int Input, int Output, Random RND) : base(Input, Output, RND)
        {
        }

        public new double[] Result(double[] Input)
        {
            int tCount = Environment.ProcessorCount;
            LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(tCount - 1);

            TaskFactory factory = new TaskFactory(lcts);
            CancellationTokenSource cts = new CancellationTokenSource();

            // Use our factory to run a set of tasks.
            double[] result = new double[OCount];
            Task[] tasks = new Task[OCount];

            lock(W)
            {
                lock (B)
                {
                    for (int k = 0; k < OCount; k++)
                    {
                        int iteration = k;

                        tasks[k] = factory.StartNew(() =>
                        {
                            double tResult = default;

                            for (int i = 0; i < ICount; i++)
                            {
                                tResult += Input[i] * W[i, iteration];
                            }

                            tResult = FunctionActivationReLu(tResult + B[iteration]);

                            lock (result)
                            {
                                result[iteration] = tResult;
                            }
                        }, cts.Token);
                    }

                    Task.WaitAll(tasks);
                }
            }

            cts.Dispose();
            return result;
        }
    }

    public class LearnData
    {
        public double[] de_dh;
        public double[] de_dt;

        public double[,] de_dw;
        public double[] de_db;
        public double[] de_dx;

        public LearnData(double[] de_dh, double[] de_dt, double[,] de_dw, double[] de_db, double[] de_dx)
        {
            this.de_dh = de_dh;
            this.de_dt = de_dt;

            this.de_dw = de_dw;
            this.de_db = de_db;
            this.de_dx = de_dx;
        }

        public LearnData()
        {
            
        }
    }
}
