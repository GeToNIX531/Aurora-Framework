using Buffers;
using MathNet.Numerics.LinearAlgebra;
using Newtonsoft.Json;
using System;
using System.IO;

namespace AI_Aurora_V1.Modules.AI.Base
{
    public class Client
    {
        [JsonProperty("Layers")]
        protected Layer[] layers { get; set; }
        public int LCount { get; private set; }
        public int ICount { get; private set; }
        public int OCount { get; private set; }

        public Client(params int[] Count)
        {
            int count = Count.Length;
            LCount = count - 1;

            if (count < 2)
                throw new Exception("Меньше 2 слоёв в ИИ");

            ICount = Count[0];
            OCount = Count[LCount];

            layers = new Layer[LCount];
            for (int i = 1; i < count; i++)
            {
                int iCount = Count[i - 1];
                int oCount = Count[i];

                layers[i - 1] = new Layer(iCount, oCount);
            }
        }

        public Vector<double> Output(Vector<double> Input)
        {
            var temp = Input;
            for (int i = 0; i < LCount; i++)
                temp = layers[i].Out(temp);

            return temp.ReLuValue();
        }

        public double Error(Vector<double> Input, Vector<double> Expected)
        {
            var r = Output(Input);
            var error = r - Expected;
            double err = error * error;
            return Activators.RMod(err);
        }

        public void Learning(Vector<double> Input, Vector<double> Output, double Alpha = 0.0001d)
        {
            lock (layers)
            {
                BPointer<Vector<double>> buffer = new BPointer<Vector<double>>(LCount + 1);
                buffer.Add(Input);

                for (int i = 0; i < LCount - 1; i++)
                {
                    buffer.Last(out var bufferValue);

                    var output = layers[i].Out(bufferValue);
                    buffer.Add(output);
                }

                var data = layers[LCount - 1].Backward(buffer.Take(), Output);
                layers[LCount - 1].Update(data);

                for (int i = 1; i < LCount; i++)
                {
                    int index = LCount - 1 - i;
                    data = layers[index].BackwardHidden(buffer.Take(), data);
                    layers[index].Update(data, Alpha);
                }
            }
        }

        public void Save(string Name)
        {
            string text = JsonConvert.SerializeObject(this);
            File.WriteAllText(Name, text);
        }

        public Client Load(string Name)
        {
            string text = File.ReadAllText(Name);
            return JsonConvert.DeserializeObject<Client>(text);
        }
    }
}
