using Aurora_Framework.Modules.AI.Base.BaseV2._1;
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

namespace Aurora_Framework.Modules.AI.BaseV2_1.Test
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

        Client aiClient = new Client(1, 5, 5, 5, 3);
        private void Find50_Load(object sender, EventArgs e)
        {
            io = new IOModify(1, 3);
            chart1.Series[0].Name = "Loss";
            chart1.Series.Add("=50");
            chart1.Series.Add(">50");
            chart1.Series.Add("<50");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double value = (int)(rnd.NextDouble() * 100);
            io.AddOutput(value == 50 ? 1 : 0);
            io.AddOutput(value > 0 ? 1 : 0);
            io.AddOutput(value < 0 ? 1 : 0);

            io.AddInput(value / 100d);

            var loss = aiClient.LossV2(io.Input, io.Output);
            double[] result = aiClient.Loss(io.Input, io.Output);
            double loss1 = result[0];
            double loss2 = result[1];
            double loss3 = result[2];
            aiClient.Learn(io.Input, io.Output, 0.01d);

            io.Clear();
        }

        int index = -1;
        private void timer2_Tick(object sender, EventArgs e)
        {
            index = index + 1;

            double value = (index % 100);

            io.AddOutput(value == 50 ? 1 : 0);
            io.AddOutput(value > 0 ? 1 : 0);
            io.AddOutput(value < 0 ? 1 : 0);


            io.AddInput(value / 100d);

            double[] result = aiClient.Loss(io.Input, io.Output);

            chart1.Series[0].Points.AddXY(index, aiClient.LossV2(io.Input, io.Output));
            io.Clear();


            chart1.Series[1].Points.AddXY(index, result[0]);
            chart1.Series[2].Points.AddXY(index, result[1]);
            chart1.Series[3].Points.AddXY(index, result[2]);

            double loss1 = result[0];
            double loss2 = result[1];
            double loss3 = result[2];

            richTextBox1.AppendText($"{value}:" + "\r\n");
            richTextBox1.AppendText($"{loss1}" + "\r\n");
            richTextBox1.AppendText($"{loss2}" + "\r\n");
            richTextBox1.AppendText($"{loss3}" + "\r\n");

            richTextBox1.AppendText(string.Empty + "\r\n");
        }
    }
}
