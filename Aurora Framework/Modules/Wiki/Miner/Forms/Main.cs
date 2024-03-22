using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.Wiki.V1._0.Forms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            client = new Miner.Client();
        }

        Miner.Client client;
        private async void button1_Click(object sender, EventArgs e)
        {
            await client.Search(richTextBox1.Text);
        }
    }
}
