using Modules.Wiki;
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

namespace Aurora_Framework.Modules.AI.LLM.Forms
{
    public partial class DataMiner : Form
    {
        public DataMiner()
        {
            InitializeComponent();

            if (Directory.Exists("Wiki/") == false)
                Directory.CreateDirectory("Wiki/");
        }

        Simple miner = new Simple();
        private void button1_Click(object sender, EventArgs e)
        {
            string start = textBox1.Text;
            miner.Start(start);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles("Wiki/");
            foreach (var file in files)
                if (richTextBox1.Text.Contains(file) == false)
                    richTextBox1.AppendText(file + "\r\n");
        }
    }
}
