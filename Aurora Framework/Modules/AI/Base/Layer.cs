using MathNet.Numerics.LinearAlgebra;
using System.Linq;

namespace AI_Aurora_V1.Modules.AI.Base
{
    public class Layer
    {
        public Matrix<double> W;
        public Vector<double> B;

        private int ICount;
        private int OCount;
        public Layer(int ICount, int OCount)
        {
            this.ICount = ICount;
            this.OCount = OCount;
            Init();
        }

        private void Init()
        {
            B = Vector<double>.Build.Random(OCount, OCount * ICount);
            W = Matrix<double>.Build.Random(ICount, OCount, OCount * ICount);
        }

        public Vector<double> Out(Vector<double> Input)
        {
            var t = Input * W + B;
            t = t.SoftMax();
            return Vector<double>.Build.DenseOfArray(t.Select(Activators.ReLu).ToArray());
        }

        public void Update(LBack Data, double Aplha = 0.001f)
        {
            W = W - Aplha * Data.de_dW;
            B = B - Aplha * Data.de_dt;
        }

        public LBack Backward(Vector<double> Input, Vector<double> Result)
        {
            var t1 = Input * W + B;
            t1 = t1.SoftMax();
            var de_dt = ICount * (t1 - Result);

            var de_dW = Matrix<double>.Build.DenseOfColumnVectors(Input) * Matrix<double>.Build.DenseOfRowVectors(de_dt);
            var de_dh = de_dt * W.Transpose();
            return new LBack(de_dt, de_dW, de_dh);
        }

        public LBack BackwardHidden(Vector<double> Input, LBack Data)
        {
            var t1 = Input * W + B;
            var t1_2 = t1.Select(Activators.ReLu_deriv).ToArray();

            int count = Data.de_dh.Count;
            for (int i = 0; i < count; i++)
                Data.de_dh[i] *= t1_2[i];

            var de_dt = Data.de_dh;
            var de_dW = Matrix<double>.Build.DenseOfColumnVectors(Input) * Matrix<double>.Build.DenseOfRowVectors(de_dt);

            var de_dh = de_dt * W.Transpose();
            return new LBack(de_dt, de_dW, de_dh);
        }

        public class LBack
        {
            public Vector<double> de_dt;
            public Matrix<double> de_dW;
            public Vector<double> de_dh;
            public Vector<double> de_db => de_dt;

            public LBack(Vector<double> de_dt, Matrix<double> de_dW, Vector<double> de_dh)
            {
                this.de_dt = de_dt;
                this.de_dW = de_dW;
                this.de_dh = de_dh;
            }
        }
    }
}
