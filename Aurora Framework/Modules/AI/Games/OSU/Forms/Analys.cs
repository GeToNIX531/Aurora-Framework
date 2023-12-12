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

namespace Aurora_Framework.Modules.AI.Games.OSU.Forms
{
    public partial class Analys : Form
    {
        public Analys()
        {
            InitializeComponent();
        }

        string[] filesData;
        FrameData[] datas;

        private void Analys_Load(object sender, EventArgs e)
        {
            filesData = Directory.GetFiles("Tracker", "*.data");
            label2.Text = filesData.Length.ToString();
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

            List<FrameData> datas = new List<FrameData>();
            foreach (var file in filesData)
            {
                var text = File.ReadAllText(file);
                var data = JsonConvert.DeserializeObject<FrameData[]>(text);

                datas.AddRange(data);
            }

            List<Game> games = new List<Game>();
            long lastScore = 0;

            Game game = new Game();
            for (int i = 0; i < datas.Count; i++)
            {
                var data = datas[i];

                if (data.OsuPPCounter == null)
                {
                    games.Add(game);
                    game = new Game();
                    lastScore = 0;
                    continue;
                }

                long score = data.OsuPPCounter.gameplay.score;

                if (score >= lastScore && score != 0)
                {
                    game.score = score;
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (Directory.Exists($"{directory}/Temp2") == false)
                Directory.CreateDirectory($"{directory}/Temp2");

            string[] files = Directory.GetFiles($"{directory}/Temp1");
            List<Game> datas = new List<Game>();

            int index = 0;
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                var temp = JsonConvert.DeserializeObject<Game>(text);

                var frames = temp.frames;
                long max = temp.score;

                //Удаляем всё что привело к ошибки
                int badCount = temp.frames.RemoveAll(T => T.OsuPPCounter.gameplay.score == max);

                var nbn = temp.NoteByNote();
                text = JsonConvert.SerializeObject(nbn);
                File.WriteAllText($"{directory}/Temp2/{index}.ds", text);
                index++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }

    public class Game
    {
        public long score;
        public List<FrameData> frames;

        public Game() => frames = new List<FrameData>();

        public CNoteByNote[] NoteByNote()
        {
            List<CNoteByNote> result = new List<CNoteByNote>();

            long score = frames[0].OsuPPCounter.gameplay.score;

            CNoteByNote temp = new CNoteByNote(score);
            for (int i = 0; i < frames.Count; i++)
            {
                var data = frames[i];
                if (data.OsuPPCounter.gameplay.score == score)
                {
                    temp.Frames.Add(data);
                }
                else
                {
                    score = data.OsuPPCounter.gameplay.score;
                    result.Add(temp);
                    temp = new CNoteByNote(score);
                }
            }

            return result.ToArray();
        }

        public class CNoteByNote
        {
            public long score;
            public List<FrameData> Frames = new List<FrameData>();

            public CNoteByNote(long Score) => score = Score;
        }
    }
}
