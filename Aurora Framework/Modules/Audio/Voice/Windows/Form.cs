using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Speech.Synthesis;
using Aurora_Framework.Modules.Audio.Voice.Windows;


namespace Aurora_Framework.Modules.AI.Voice.Windows
{
    public partial class Form : System.Windows.Forms.Form
    {
        Client client;
        public Form()
        {
            InitializeComponent();
            client = new Client();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Speak(textBox1.Text);
        }
    }
}
