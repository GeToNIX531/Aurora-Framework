using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hash
{
    public static class TTHash
    {
        public static string TTHashGet(this string Text, int Size = 54)
        {
            var now = DateTime.UtcNow.ToBinary();

            long result = 0;
            bool plus = true;
            foreach (int value in Text)
            {
                if (plus)
                    result += value;
                else result -= value;

                plus = !plus;
            }

            while (result.ToString().Length > Size)
            {
                result <<= 2;
            }

            string resultV2 = string.Empty;

            if (result < 0) result = -result;
            var temp = result.ToString();

            foreach (int value in temp)
            {
                resultV2 += (char)value;
            }

            temp = now.ToString();
            resultV2 += '-';
            foreach (int value in temp)
            {
                resultV2 += (char)value;
            }

            return $"{resultV2}";
        }
    }
}
