using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Main.Analyser.Text.Forms
{
    public partial class Finder : Form
    {
        string[] rusLang;
        public Finder()
        {
            InitializeComponent();
            rusLang = File.ReadAllLines("Lang/russian.txt", Encoding.GetEncoding(1251));
            richTextBox1.MaxLength = int.MaxValue;
        }

        int lastCount = -1;
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            string text = richTextBox1.Text;
            int count = text.Length;

            if (count <= 3) return;

            long start = 0;
            long end = rusLang.Length;

            int step = 0;
            do {
                long temp = (start + end) / 2;

                string t = rusLang[temp].ToLower();

                for (var i = 0; i < count && i < t.Length; i++)
                {
                    step++;
                    int tx1 = (int)t[i];
                    int tx2 = (int)text[i];

                    if (tx1 == tx2) continue;
                    else
                    {
                        if (tx1 > tx2)
                        {
                            end = temp;
                            var value = rusLang.Length % (10000 / (step + 1));
                            if (start - value >= 0)
                            {
                                start -= value;
                            }
                        }
                        else
                        {
                            start = temp;
                            var value = rusLang.Length % (10000 / (step + 1));
                            if (end + value <= rusLang.Length)
                                end += value;
                        }

                        break;
                    }

                    
                }
            }
            while (step <= 10000);

            richTextBox2.Clear();

            label1.Text = (end - start).ToString();
            for (int i = 0; start < end && i < 1000; start++)
            {
                var value = rusLang[start];
                richTextBox2.AppendText(value + "\n");
                if (i % 1000 == 0)
                    richTextBox1.Invalidate();
            }

            lastCount = count;
        }
    }
}
