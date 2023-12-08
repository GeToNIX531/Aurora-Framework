using Buffers;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI_Aurora_V1.Data.Vector
{
    public static class Extension
    {

        #region Превращение Одиночных элементов в массив и вектора
        public static T[] OneToArray<T>(this T Value) => new T[] { Value };
        public static T[] OneToArray<T>(params T[] Value) => Value;

        public static Vector<T> OneToVector<T>(this T Value) where T : struct, IEquatable<T>, IFormattable
        {
            var array = Value.OneToArray();
            return array.ToVector();

        }

        #endregion

        public static T[] ToArray<T>(this BPointer<T> Buffer)
        {
            T[] array = new T[Buffer.Size];
            int count = Buffer.Size;

            for (int i = 0; i < count; i++)
            {
                T value = Buffer.Take();
                int point = Buffer.Point;
                array[point] = value;
            }

            return array;
        }

        public static Vector<int> CharToVector(this int Value)
        {
            string v = Value.ToString();
            int count = v.Length;
            int[] array = new int[count];

            for (int i = 0; i < v.Length; i++)
            {
                var ch = v[i].ToString();
                int value = int.Parse(ch);
                array[i] = value;
            }

            return Vector<int>.Build.DenseOfArray(array);
        }

        public static Vector<float> CharToVectorF(this int Value, int Size)
        {
            string v = Value.ToString();
            int count = v.Length;
            float[] array = new float[Size];

            for (int i = 0; i < count; i++)
            {
                int index = Size - count + i - 1;
                var ch = v[i].ToString();
                int value = int.Parse(ch);
                array[index] = value;
            }

            return Vector<float>.Build.DenseOfArray(array);
        }

        #region Создание вектора из массива

        public static Vector<T> ToVector<T>(this T[] Array) where T : struct, IEquatable<T>, IFormattable
        {
            return Vector<T>.Build.DenseOfArray(Array);
        }

        public static Vector<double> ToVector<T>(this int[] Array) where T : struct, IEquatable<double>, IFormattable
        {
            int count = Array.Length;
            double[] array = new double[count];
            for (int i = 0; i < count; i++)
                array[i] = Array[i];

            return array.ToVector();
        }

        public static Vector<T> ToVector<T>(this IEnumerable<T> Array) where T : struct, IEquatable<T>, IFormattable =>
            Vector<T>.Build.DenseOfArray(Array.ToArray());

        #endregion

        public static Vector<T> ToVector<T>(this T[] Array, int Size) where T : struct, IEquatable<T>, IFormattable
        {
            int count = Size;
            T[] array = new T[count];
            for (int i = 0; i < Array.Length; i++)
                array[i] = Array[i];

            return Vector<T>.Build.DenseOfArray(array);
        }
    }
}
