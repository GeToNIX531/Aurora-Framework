using Aurora_Framework.Modules.AI.BaseV2.Data;
using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.AI.BaseV2.Test
{
    public partial class Math : Form
    {
        private IOModify io;
        private Random rnd;
        public Math()
        {
            rnd = new Random();
            InitializeComponent();
        }

        BaseV2.Client aiClient = new Client(1, 10, 2);
        private void Find50_Load(object sender, EventArgs e)
        {
            io = new IOModify(1, 2);
            chart1.Series.Add(">50");
            chart1.Series.Add("<50");
            chart1.Series.Add("=50");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double value = (int)(rnd.NextDouble() * 100);
            value = System.Math.PI * value;
            io.AddInput(value);

            io.AddOutput(value % 2 == 0 ? 1 : 0);
            io.AddOutput(value % 2 == 0 ? 0 : 1);

            for (int i = 0; i < 100; i++)
            {
                aiClient.Learn(io.Input, io.Output);
            }

            io.Clear();
        }

        int index = -1;
        private void timer2_Tick(object sender, EventArgs e)
        {
            index = index + 1;

            double value = rnd.NextDouble() * 100;
            value = (int)(System.Math.PI * value);
            io.AddInput(value);

            io.AddOutput(value % 2 == 0 ? 1 : 0);
            io.AddOutput(value % 2 == 0 ? 0 : 1);

            double[] result = aiClient.Loss(io.Input, io.Output);
            io.Clear();

            chart1.Series[1].Points.AddXY(index, result[0]);
            chart1.Series[2].Points.AddXY(index, result[1]);

            double loss1 = result[0];
            double loss2 = result[1];

            if (System.Math.Abs(loss1) >= 0.1 || System.Math.Abs(loss2) >= 0.1)
            {

                richTextBox1.AppendText($"{value}:" + "\r\n");
                richTextBox1.AppendText($"{loss1}" + "\r\n");
                richTextBox1.AppendText($"{loss2}" + "\r\n");

                richTextBox1.AppendText(string.Empty + "\r\n");
            }
        }
    }
}
