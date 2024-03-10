using AI_Aurora_V1.Data.Vector;
using Aurora_Framework.Modules.AI.Games.OSU.Data;
using MathNet.Numerics.LinearAlgebra;
using SimWinInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FastHooker.WinAPI;


namespace Aurora_Framework.Modules.AI.Games.OSU.Analyser.Forms
{
    public partial class Main : Form
    {
        private Viewer viewer;
        public Main()
        {
            this.StartPosition = FormStartPosition.Manual;



            InitializeComponent();
            Start();

            viewer = new Viewer();
            viewer.Show();
        }

        OsuPPCounter ppCounter;
        AI_Aurora_V1.Modules.AI.Base.Client aiClient;

        public void Start()
        {
            Task.Run(async () =>
            {
                #region Загрузка DirectX и рендера
                Rectangle sFull = new Rectangle(0, 0, 1920, 1080);
                Rectangle sSmall = new Rectangle(0, 0, 80, 60);

                Bitmap small = new Bitmap(sSmall.Width, sSmall.Height);
                Graphics gSmall = Graphics.FromImage(small);

                var directX = new DirectX(sFull.Size);
                int w = small.Width;
                int h = small.Height;

                float avg = 1 / 3f;
                float fColor = 1 / 256f;


                viewer.ViewerUpdate(small);
                #endregion

                ppCounter = new OsuPPCounter();

                int iCount = sSmall.Width * sSmall.Height;
                aiClient = new AI_Aurora_V1.Modules.AI.Base.Client(iCount, iCount / 8, iCount / 16,  4);
                Random rnd = new Random();

                var fHooker = new FastHooker();
                bool aClick = false;
                bool bClick = false;

                fHooker.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.V) aClick = true;
                    if (e.KeyCode == Keys.B) bClick = true;
                };
                fHooker.KeyUp += (s, e) =>
                {
                    if (e.KeyCode == Keys.V) aClick = false;
                    if (e.KeyCode == Keys.B) bClick = false;
                };


                int count = 1000;


                Data.Input input = null;
                Output Output = null;
                Point cursor = Point.Empty;

                IntPtr hwid = User32.FindWindow("WindowsForms10.Window.2b.app.0.1d2098a_r8_ad1", null);
                var osu = Process.GetProcessesByName("osu!")[0];


                List<DB> db = new List<DB>();

                int combo = 0;
                int errorCount = 0;
                while (true)
                {
                    viewer.ViewerInvalidate();

                    if (directX.TakeScreenshot(out Bitmap image))
                    {
                        gSmall.DrawImage(image, sSmall, sFull, GraphicsUnit.Pixel);


                        if (ppCounter.Update(out var data) == false) continue;
                        if (data.menu.state != 2) continue;
                        if (data.menu.bm.time.current < 0) continue;
                        if (data.menu.bm.time.current >= data.menu.bm.time.mp3) continue;

                        cursor = Cursor.Position;

                        if (input != null && Output != null)
                            db.Add(new DB(sFull.Size, input, Output));

                        if (combo < data.gameplay.combo.current)
                        {
                            var x = db[db.Count - 1].X;
                            var y = db[db.Count - 1].Y;

                            var xdelta = x - db[0].X;
                            var ydelta = y - db[0].Y;

                            double distance = Math.Sqrt(xdelta * xdelta + ydelta * ydelta);
                            for (int i = 1; i < db.Count - 1 && i < 120; i++)
                            {
                                xdelta = x - db[i].X;
                                ydelta = y - db[i].Y;
                                var newDistance = Math.Sqrt(xdelta * xdelta + ydelta * ydelta);

                                if (distance > newDistance)
                                {
                                    var inp = db[i].InputValues;
                                    var outp = db[i].OutputValues;
                                    Task.Run(() => aiClient.Learning(inp.ToVector<double>(), outp.ToVector<double>(), +0.01d));
                                }

                                distance = newDistance;
                            }

                            db.Clear();
                        }

                        if (combo > data.gameplay.combo.current)
                            db.Clear();

                        if (data.gameplay.hits.hitErrorArray == null)
                        {
                            errorCount = 0;
                        }
                        else
                        {
                            if (errorCount > data.gameplay.hits.hitErrorArray.Length)
                            {
                                if (cursor.X < 2 || cursor.X > 1920 - 2 || cursor.Y > 1280 - 2 || cursor.Y < 2)
                                {
                                    var i = db[db.Count - 1].InputValues;
                                    var o = db[db.Count - 1].OutputValues;
                                    Task.Run(() => aiClient.Learning(i.ToVector<double>(), o.ToVector<double>(), -0.00001d));
                                }

                                db.Clear();
                            }

                            errorCount = data.gameplay.hits.hitErrorArray.Length;
                        }

                        combo = data.gameplay.combo.current;


                        float[] values = new float[w * h];

                        int index = 0;
                        for (int y = 0; y < h; y++)
                            for (int x = 0; x < w; x++)
                            {
                                var p = small.GetPixel(x, y);
                                int value = p.R + p.G + p.B;
                                value = (int)(value * avg);

                                small.SetPixel(x, y, Color.FromArgb(value, value, value));
                                values[index] = value * fColor;
                                index++;
                            }


                        input = new Data.Input(values, cursor);
                        Output = new Output(cursor, aClick, bClick);

                        var inputLayer = input.Get(sFull.Size).ToVector();
                        var output = aiClient.Output(inputLayer);

                        /*
                        var aClickRND = rnd.NextDouble();
                        var bClickRND = rnd.NextDouble();
                        */

                        var _x = (int)((output[2]) * 1920d);
                        var _y = (int)((output[3]) * 1280d);
                        frame++;

                        //SimMouse.Act(SimMouse.Action.MoveOnly, _x, _y);

                        if (count > 0)
                        {
                            count--;
                        }
                        else SimMouse.Act(SimMouse.Action.MoveOnly, _x, _y);

                        //Task.Run(() => User32.SetCursorPos(xPost, yPost));

                        /*
                        if (xPost == 0 && yPost == 0) continue;

                        UInt32 WM_MOUSEMOVE = 0x0200;
                        Task.Run(()=> User32.SendMessage(osu.MainWindowHandle, WM_MOUSEMOVE, IntPtr.Zero, MakeLParam(xPost, yPost)));
                        */
                    }
                }
            });
        }

        int frame = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            int f = frame;
            Text = $"{f}";
            frame -= f;
        }

        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
    }
}
