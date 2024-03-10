using AI_Aurora_V1.Data.Vector;
using Aurora_Framework.Modules.AI.Games.OSU.Data;
using Newtonsoft.Json;
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

namespace Aurora_Framework.Modules.AI.Games.OSU
{
    public partial class Analyse : Form
    {
        public Analyse()
        {
            InitializeComponent();
        }

        string directory = "Data sets/OSU";
        private void button1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists("Data sets") == false)
                Directory.CreateDirectory("Data sets");

            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);

            if (Directory.Exists($"{directory}/Temp1") == false)
                Directory.CreateDirectory($"{directory}/Temp1");

            List<OsuData> datas = new List<OsuData>();
            foreach (var file in filesData)
            {
                var text = File.ReadAllText(file);
                var data = JsonConvert.DeserializeObject<OsuData[]>(text);

                datas.AddRange(data);
            }

            List<Game> games = new List<Game>();
            Game game = new Game();
            long lastScore = 0;

            for (int i = 0; i < datas.Count; i++)
            {
                var data = datas[i];

                if (data.Data == null)
                {
                    games.Add(game);
                    game = new Game();
                    lastScore = 0;
                    continue;
                }

                long score = data.Data.gameplay.score;

                if (score >= lastScore && score != 0)
                {
                    game.MaxScore = score;
                    game.frames.Add(data);
                    lastScore = score;
                }
                else
                {
                    games.Add(game);
                    game = new Game();
                    lastScore = 0;
                }
            }

            int index = 0;
            foreach (var value in games)
            {
                if (value.frames.Count == 0) continue;

                var text = JsonConvert.SerializeObject(value);
                File.WriteAllText($"{directory}/Temp1/{index}.ds", text);
                index++;
            }

            label1.Text = (index).ToString();
        }

        string[] filesData;
        private void Analyser_Load(object sender, EventArgs e)
        {
            filesData = Directory.GetFiles("Tracker", "*.fd");
            label2.Text = filesData.Length.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists($"{directory}/Temp2") == false)
                Directory.CreateDirectory($"{directory}/Temp2");

            string[] files = Directory.GetFiles($"{directory}/Temp1");
            List<Game> datas = new List<Game>();

            int index = 0;
            foreach (var file in files)
            {
                index++;
                var text = File.ReadAllText(file);
                var temp = JsonConvert.DeserializeObject<Game>(text);

                var frames = temp.frames;
                long max = temp.MaxScore;

                //Удаляем всё что привело к ошибки
                int badCount = temp.frames.RemoveAll(T => T.Data.gameplay.score == max);


                var nbn = temp.NoteByNote();
                if (nbn == null) continue;

                text = JsonConvert.SerializeObject(nbn);
                File.WriteAllText($"{directory}/Temp2/{index - 1}.ds", text);
            }
        }

        const int inputCount = 80 * 60 + 2;
        AI_Aurora_V1.Modules.AI.Base.Client aiclient = new AI_Aurora_V1.Modules.AI.Base.Client(inputCount, inputCount, 4);
        private void button3_Click(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles($"{directory}/Temp2");

            int xMax = 1920;
            int yMax = 1080;

            float xFMax = 1 / xMax;
            float yFMax = 1 / yMax;

            float xMaxCursorMove = 1 / 100f;
            float yMaxCursorMove = 1 / 100f;


            long progress = 0;
            progressBar1.Maximum = files.Length;
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                CNoteByNote[] data = JsonConvert.DeserializeObject<CNoteByNote[]>(text);

                foreach (var value in data)
                {
                    var frames = value.Frames;

                    double loss = 0;
                    for(int i = 0; i < frames.Count - 1; i++)
                    {
                        var frame = frames[i];

                        #region input
                        var screen = frame.Screen;

                        int xCount = 80;
                        int yCount = 60;

                        int sCount = xCount * yCount;
                        int cursorCount = 1 + 1; //x < 0.5: x(-) ? x(+)

                        int iCount = sCount + cursorCount;

                        double[] input = new double[iCount];

                        int index = 0;
                        for (int x = 0; x < xCount; x++)
                            for (int y = 0; y < yCount; y++)
                            {
                                input[index] = screen[x, y];
                                index++;
                            }

                        int xCursor = frame.CursorPosition.X;
                        int yCursor = frame.CursorPosition.Y;

                        input[index] = xCursor * xFMax;
                        index++;

                        input[index] = yCursor * yFMax;
                        #endregion

                        #region Output

                        //Cursor(x,y), Buttons(1,2)
                        double[] output = new double[4];


                        #endregion

                        var cursor = frames[i + 1].CursorPosition;
                        var aDown = frame.aDown;
                        var bDown = frame.bDown;

                        var xDelta = cursor.X - frame.CursorPosition.X;
                        var yDelta = cursor.Y - frame.CursorPosition.Y;

                        var oXDelta = xDelta * xMaxCursorMove;
                        var oYDelta = yDelta * yMaxCursorMove;

                        oXDelta = oXDelta + xMaxCursorMove;
                        oXDelta = oXDelta * 0.5f;

                        oYDelta = oYDelta + yMaxCursorMove;
                        oYDelta = oYDelta * 0.5f;

                        output[0] = oXDelta;
                        output[1] = oYDelta;

                        output[2] = aDown == true ? 1 : 0;
                        output[3] = bDown == true ? 1 : 0;

                        aiclient.Learning(input.ToVector(), output.ToVector());
                        progress++;
                        label6.Text = progress.ToString();

                        loss += aiclient.Error(input.ToVector(), output.ToVector());

                        Update();
                    }

                    label5.Text = string.Format("{0:0.00}", loss / frames.Count * 4);
                    Update();
                }

                progressBar1.Value += 1;
                Task.Delay(1);
                Invalidate();
                progressBar1.Invalidate();
            }

            aiclient.Save("OsuAIModel.mdl");
        }
    }
}
