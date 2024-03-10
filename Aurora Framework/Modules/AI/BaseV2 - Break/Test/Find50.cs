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
    public partial class Find50 : Form
    {
        public Find50()
        {
            InitializeComponent();
        }

        BaseV2.Client aiClient = new Client(1, 3, 3);
        private void Find50_Load(object sender, EventArgs e)
        {
            chart1.Series.Add(">50");
            chart1.Series.Add("<50");
            chart1.Series.Add("=50");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var data = AI_Aurora_V1.Modules.AI.Base.TrainData.Find50.Get();
            aiClient.Learn(data.Input.ToArray(), data.Outputs.ToArray());
        }

        int index = -1;
        int value = -1;
        private void timer2_Tick(object sender, EventArgs e)
        {
            index = index + 1;
            value = value + 1;
            if (value >= 100) value = 0;

            Vector<double> input = Vector<double>.Build.Dense(new double[] { value });
            Vector<double> output = AI_Aurora_V1.Modules.AI.Base.TrainData.Find50.Get(value);

            double[] result = aiClient.Result(input.ToArray());
            chart1.Series[1].Points.AddXY(index, result[0]);
            chart1.Series[2].Points.AddXY(index, result[1]);
            chart1.Series[3].Points.AddXY(index, result[2]);
        }
    }
}
