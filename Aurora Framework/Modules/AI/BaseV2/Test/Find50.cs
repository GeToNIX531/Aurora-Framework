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

        BaseV2.Client aiClient = new Client(1, 2, 3, 3);
        private void Find50_Load(object sender, EventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var data = AI_Aurora_V1.Modules.AI.Base.TrainData.Find50.Get();
            aiClient.Learn(data.Input.ToArray(), data.Outputs.ToArray());
            this.Text = aiClient.Loss(data.Input.ToArray(), data.Outputs.ToArray()).ToString();
        }
    }
}
