using AI_Aurora_V1.Data.Vector;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AI_Aurora_V1.Modules.AI.Base
{
    public static class Activators
    {

        public static Func<double, double> RMod = new Func<double, double>(T => Math.Min(1, T));
        public static Func<double, double> ReLu = new Func<double, double>(T => Math.Max(0, T));

        public static Func<double, double> ReLu_deriv = new Func<double, double>(T => T >= 0 ? 1 : 0);

        public static Vector<double> SoftMax(this Vector<double> Vector)
        {
            IEnumerable<double> array = Vector.ToArray();
            IEnumerable<double> result = array.Select(T => Math.Exp(T));
            double sum = result.Sum();

            if (sum != 0 && Math.Abs(sum) < double.MaxValue)
                result = result.Select(T => T / sum);

            IEnumerable<double> output = result.Select(T => (double)T);
            return output.ToVector();
        }

        public static Vector<double> ReLuValue(this Vector<double> Vector)
        {
            IEnumerable<double> array = Vector.ToArray();
            IEnumerable<double> result = array.Select(ReLu);
            return result.ToVector();
        }
    }
}
