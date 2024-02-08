using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.AI.Games.OSU.Analyser.Forms
{
    public partial class Viewer : Form
    {
        public Viewer()
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(1920 - Width / 2, Height / 8);
            InitializeComponent();
        }

        public void ViewerInvalidate()
        {
            pictureBox1.Invalidate();
        }
        public void ViewerUpdate(Bitmap Image) => pictureBox1.Image = Image;
    }
}
