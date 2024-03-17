using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.Pythone
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            logger = new Logger(Color.Green);
            logger.OutputEvent += LogHandler;
        }

        private Test client;
        private Logger logger;
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            client = new Test
            {
                Logger = logger
            };
            client.Run();
        }
        private void LogHandler(string Text, Color Color)
        {
            if (string.IsNullOrEmpty(Text)) return;
            richTextBox1.Invoke((Action)delegate {
                var color = richTextBox1.SelectionColor;

                richTextBox1.SelectionColor = Color;
                richTextBox1.AppendText(Text + "\r\n");
                richTextBox1.SelectionColor = color;
            });
        }

        Script pythonScript;
        private void button3_Click(object sender, EventArgs e)
        {
            pythonScript = new Script(100);
            var lines = textBox2.Lines;
            pythonScript.Add(lines);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pythonScript.Save("test1");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pythonScript.Run(logger);
        }
    }
}
