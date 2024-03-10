using System.Linq;
using Mathimatic = System.Math;

namespace Aurora_Framework.Main.Math
{
    public static class FunctionActivation
    {
        public static double ReLu(double Value) => Mathimatic.Max(0, Value);
        public static double[] SoftMax(double[] Values)
        {
            var z_exp = Values.Select(Mathimatic.Exp);

            var sum_z_exp = z_exp.Sum();

            double coeficent = 1 / sum_z_exp;
            var softmax = z_exp.Select(i => i * coeficent);
            return softmax.ToArray();
        }

        public static double[] ReLu(double[] Values) => Values.Select(T => Mathimatic.Max(0, T)).ToArray();
    }
}
