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

namespace Aurora_Framework.Modules.AI.Games.OSU.Forms
{
    public partial class Tracker : Form
    {
        public TrackerUpdateStatus UpdateStatusEvent;
        public delegate void TrackerUpdateStatus(bool Status);

        OsuPPCounter counter;
        protected readonly GlobalHook hook = new GlobalHook();
        public Tracker()
        {
            counter = new OsuPPCounter();

            data = new List<FrameData>();
            CurrentFrame = new FrameData(0, Point.Empty);
            InitializeComponent();

            hook.KeyDown += KeyDownHandler;
            hook.KeyUp += KeyUpHandler;

            if (Directory.Exists("Tracker/") == false)
                Directory.CreateDirectory("Tracker/");

            this.TopMost = true;
            UpdateStatus(false);
        }

        private List<FrameData> data;
        private int Count;

        private void KeyUpHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V)
                button1.BackColor = Color.White;

            if (e.KeyCode == Keys.B)
                button2.BackColor = Color.White;

            CurrentFrame.Status = false;
        }

        private bool Status = false;
        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V)
                button1.BackColor = Color.Green;

            if (e.KeyCode == Keys.B)
                button2.BackColor = Color.Green;

            if (e.KeyCode == Keys.K) UpdateStatus(!Status);

            CurrentFrame.Buttons = e.KeyCode;
            CurrentFrame.Status = true;
        }

        public void UpdateStatus(bool Status)
        {
            this.Status = Status;
            UpdateStatusEvent?.Invoke(Status);

            if (Status == false && Count != 0)
            {
                data.Save<List<FrameData>>($"Tracker/{Frame}.data", out var Error);
                Count = 0;
                data.Clear();
            }
        }

        private long Frame;
        public void Tick(long frame)
        {
            Frame = frame;
            if (Status == false) return;

            data.Add(CurrentFrame);
            Count++;

            var cursor = Cursor.Position;
            Text = cursor.ToString();

            CurrentFrame = new FrameData(frame, cursor);
            counter.Update();

            if (counter.isClose == false)
            {
                var dCounter = counter.data;
                long score = dCounter.gameplay.score;
                button3.Text = score.ToString();
                CurrentFrame.OsuPPCounter = dCounter;
            }

            if (Count >= 120)
            {
                data.Save<List<FrameData>>($"Tracker/{frame}.data", out var Error);
                Count = 0;
                data.Clear();
            }
        }

        private FrameData CurrentFrame;
    }

    public class FrameData
    {
        public OsuPPCounter.Data OsuPPCounter;
        public Point CursorPosition;
        public Keys Buttons;
        public bool Status;
        public long Frame;

        public FrameData(long Frame, Point CursorPosition)
        {
            this.Frame = Frame;
            this.CursorPosition = CursorPosition;
        }
    }

    public class OsuPPCounter : IDisposable
    {
        public WebClient client;
        public OsuPPCounter()
        {
            client = new WebClient();
        }

        string url = "http://127.0.0.1:24050/json";
        public string Get() => client.DownloadString(url);

        public void Dispose()
        {
            client.Dispose();
        }

        public Data data {get; private set;}

        public bool isClose = false;
        public void Update()
        {
            string value = Get();
            if (value.Contains("error"))
            {
                isClose = true;
                return;
            }

            data = JsonConvert.DeserializeObject<Data>(value);
        }


        public class Data
        {
            public GamePlay gameplay;

            public class GamePlay
            {
                public bool gameMode;
                public string name;
                public long score;
                public float accuracy;

                public Combo combo;

                public class Combo
                {
                    public int current;
                    public int max;
                }

                public Hits hits;

                public class Hits
                {
                    [Newtonsoft.Json.JsonProperty("300")]
                    public int Score_300;
                    public int geki;

                    [Newtonsoft.Json.JsonProperty("100")]
                    public int Score_100;
                    public int katu;

                    [Newtonsoft.Json.JsonProperty("50")]
                    public int Score_50;
                    [Newtonsoft.Json.JsonProperty("0")]
                    public int Score_0;

                    public int sliderBreaks;
                }
            }
        }
    }
}
