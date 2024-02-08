using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.AI.TextGenerate
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private Client client;
        private void Main_Load(object sender, EventArgs e)
        {
            client = new Client();
            Text = client.Count.ToString();
        }
    }
}
