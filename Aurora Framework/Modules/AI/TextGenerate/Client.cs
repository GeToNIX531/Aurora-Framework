using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buffers.Tokenezator;

namespace Aurora_Framework.Modules.AI.TextGenerate
{
    public class Client
    {
        private char[] rusABC = { 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я' };


        private char[] symbols = { ',', '.', '!', '?', ' ', '\n' };

        private char[] block = { 'ъ', 'ь', 'ы' };

        private char[] number = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };


        private Inputer inputer;
        private Outputer outputer;
        public Client()
        {
            inputer  = new Inputer();

            var array = rusABC.Concat(symbols).ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                string a = array[i].ToString();
                inputer.Add(a);

                if (symbols.Contains(array[i]) || number.Contains(array[i])) continue;
                if (block.Contains(array[i])) continue;

                string A = a.ToUpper();
                inputer.Add(A);


                for (int k = i; k < array.Length; k++)
                {
                    string b = array[k].ToString();

                    inputer.Add(a + b);
                    inputer.Add(A + b);
                }
            }


            foreach (var item in number)
                inputer.Add(item.ToString());

            outputer = inputer.ToOutputer();

        }

        public int Count => inputer.Count;
    }
}
