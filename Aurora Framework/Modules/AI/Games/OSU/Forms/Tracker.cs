using Data.Saver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

using OsuHelper;
using OsuApiHelper;
using OsuApiHelper.Math;

using System.Net;
using System.Net.WebSockets;
using System.Threading;

using Newtonsoft.Json;
using Aurora_Framework.Modules.AI.Games.OSU.Data;
using System.Drawing.Imaging;

namespace Aurora_Framework.Modules.AI.Games.OSU.Forms
{
    public partial class Tracker : Form
    {
        OsuPPCounter counter;
        protected readonly FastHooker hook;

        public TrackerUpdateStatus UpdateStatusEvent;
        public delegate void TrackerUpdateStatus(bool Status);

        public Tracker()
        {
            counter = new OsuPPCounter();

            data = new List<FrameData>();
            CurrentFrame = new FrameData(0, Point.Empty, ButtonAPress, ButtonBPress);
            InitializeComponent();

            
            hook = new FastHooker();
            hook.KeyDown += KeyDownHandler;
            hook.KeyUp += KeyUpHandler;


            if (Directory.Exists("Tracker/") == false)
                Directory.CreateDirectory("Tracker/");

            this.TopMost = true;
            UpdateStatus(false);
        }

        private List<FrameData> data;
        private int Count;

        private bool ButtonAPress = false;
        private bool ButtonBPress = false;
        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;

            if (key == Keys.V)
            {
                button1.BackColor = Color.White;
                ButtonAPress = true;
            }
            else if (key == Keys.B)
            {
                button2.BackColor = Color.White;
                ButtonBPress = true;
            }
        }

        private bool Status = false;
        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            var key = e.KeyCode;
            if (key == Keys.V)
            {
                button1.BackColor = Color.Green;
                ButtonAPress = false;
            }
            else if (key == Keys.B)
            {
                button2.BackColor = Color.Green;
                ButtonBPress = false;
            }
            else if (key == Keys.K) UpdateStatus(!Status);
        }

        public void UpdateStatus(bool Status)
        {
            this.Status = Status;
            UpdateStatusEvent?.Invoke(Status);

            if (Status == true)
            {
                button4.BackColor = Color.Green;
            }
            else
            {
                button4.BackColor = Color.White;
                if (Count != 0)
                {
                    data.Save<List<FrameData>>($"Tracker/{Frame}.data", out var Error);
                    Count = 0;
                    data.Clear();
                }
            }
        }


        private long Frame;
        public void Tick(long frame)
        {
            this.Frame = frame;
            if (Status == false) return;


            data.Add(CurrentFrame);
            Count++;

            var cursor = Cursor.Position;
            Text = cursor.ToString();

            CurrentFrame = new FrameData(frame, cursor, ButtonAPress, ButtonBPress);

            if (counter.Update(out var sData))
            {
                long score = sData.gameplay.score;
                button3.Text = score.ToString();
                CurrentFrame.OsuPPCounter = sData;
            }

            
            if (Count == 256)
            {
                data.Save($"Tracker/{frame}.data", out var Error);
                Count = 0;
                data.Clear();
            }
        }

        private FrameData CurrentFrame;
    }

    
}
