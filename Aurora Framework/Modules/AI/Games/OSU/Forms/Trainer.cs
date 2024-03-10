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
using AI_Aurora_V1.Modules.AI.Base;
using Newtonsoft.Json;


namespace Aurora_Framework.Modules.AI.Games.OSU.Forms
{
    public partial class Trainer : Form
    {
        public Trainer()
        {
            InitializeComponent();
        }

        Client aiClient;
        private void Trainer_Load(object sender, EventArgs e)
        {
        }

        Image Temp;
        string directory = "Data sets/OSU";
        private void button1_Click(object sender, EventArgs e)
        {
            var files = Directory.GetFiles($"{directory}/Temp2/");
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                var nbn = JsonConvert.DeserializeObject<Game.CNoteByNote[]>(text);

                foreach (var note in nbn)
                {
                    var frames = note.Frames;
                    foreach (var value in frames)
                    {

                    }
                }
            }
        }
    }
}
