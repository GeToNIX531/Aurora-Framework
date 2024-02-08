using Aurora_Framework.Modules.AI.Games.OSU.Data;
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
using Newtonsoft.Json;

namespace Aurora_Framework.Modules.AI.Games.OSU
{
    public partial class Client : Form
    {
        public Client()
        {
            InitializeComponent();
            TopMost = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int f = frame;
            Text = $"{f * 2}";
            frame -= f;
        }

        int frame = 0;
        private void Client_Load(object sender, EventArgs e)
        {
            FastHooker hooker = new FastHooker();
            hooker.KeyDown += keyDown;
            hooker.KeyUp += keyUp;

            ppCounter = new OsuPPCounter();

            Task.Factory.StartNew(Start);
        }
        OsuPPCounter ppCounter;

        private long frameCount;
        private void Start()
        {
            Task.Run(async () =>
            {
                Rectangle sFull = new Rectangle(0, 0, 1920, 1080);
                Rectangle sSmall = new Rectangle(0, 0, 80, 60);

                Bitmap small = new Bitmap(sSmall.Width, sSmall.Height);
                Graphics gSmall = Graphics.FromImage(small);

                var directX = new DirectX(sFull.Size);

                int w = small.Width;
                int h = small.Height;

                float avg = 1 / 3f;
                float fColor = 1 / 256f;
                Bitmap result = new Bitmap(sSmall.Width, sSmall.Height);
                pictureBox1.Image = result;

                while (true)
                {
                    pictureBox1.Invalidate();

                    try
                    {

                        if (directX.TakeScreenshot(out Bitmap image))
                        {
                            gSmall.DrawImage(image, sSmall, sFull, GraphicsUnit.Pixel);

                            frameCount++;
                            if (ppCounter.Update(out var data) == false) continue;
                            if (data.menu.state != 2) continue;
                            if (data.menu.bm.time.current < 0) continue;
                            if (data.menu.bm.time.current > data.menu.bm.time.mp3) continue;

                            float[,] values = new float[w, h];

                            lock (small)
                            {
                                for (int x = 0; x < w; x++)
                                    for (int y = 0; y < h; y++)
                                    {
                                        var p = small.GetPixel(x, y);
                                        int value = p.R + p.G + p.B;
                                        value = (int)(value * avg);

                                        result.SetPixel(x, y, Color.FromArgb(value, value, value));
                                        values[x, y] = value * fColor;
                                    }
                            }
                            var cursorPosition = Cursor.Position;

                            Task.Run(() => Save(values, cursorPosition, frameCount, data));

                            frame++;
                        }

                    }
                    catch
                    { }



                    /*
                    gFull.CopyFromScreen(Point.Empty, Point.Empty, sFull.Size);
                    gSmall.DrawImage(full, sSmall, sFull, GraphicsUnit.Pixel);

                    pictureBox1.Image = small;
                    pictureBox1.Invalidate();
                    */
                }
            });
        }

        private bool keyADown;
        private bool keyBDown;

        private void keyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V) keyADown = true;
            if (e.KeyCode == Keys.B) keyBDown = true;
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V) keyADown = false;
            if (e.KeyCode == Keys.B) keyBDown = false;
        }

        

        List<OsuData> values = new List<OsuData>();
        public void Save(float[,] Value, Point Cursor, long Frame, OsuPPCounter.Data Data)
        {

            var osuData = new OsuData(Value, Cursor, Frame, keyADown, keyBDown, Data);

            OsuData[] save = null;

            lock (values)
            {
                values.Add(osuData);

                if (values.Count >= 600)
                {
                    save = values.ToArray();
                    values.Clear();
                }
            }

            if (save != null)
            {
                string text = JsonConvert.SerializeObject(save);
                File.WriteAllText($"Tracker/{Frame}.fd", text);
            }
        }

        bool ready = true;
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (ready)
            {
                ready = false;
                Tick();
                ready = true;
            }
        }

        private void Tick()
        {
            // получаем размеры окна рабочего стола
            //Rectangle bounds = Screen.GetBounds(Point.Empty);

            //screenStateLogger.CaptureScreen(out var bitmap);
            //pictureBox1.Image = bitmap;
        }
    }
}
