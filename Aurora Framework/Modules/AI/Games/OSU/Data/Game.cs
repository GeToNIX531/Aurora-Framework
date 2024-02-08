using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.AI.Games.OSU.Data
{
    public class Game
    {
        public long MaxScore;
        public List<OsuData> frames;

        public Game() => frames = new List<OsuData>();

        public CNoteByNote[] NoteByNote()
        {
            List<CNoteByNote> result = new List<CNoteByNote>();

            if (frames.Count == 0) return null;

            long score = frames[0].Data.gameplay.score;

            CNoteByNote temp = new CNoteByNote(score);
            for (int i = 0; i < frames.Count; i++)
            {
                var data = frames[i];
                if (data.Data.gameplay.score == score)
                {
                    temp.Frames.Add(data);
                }
                else
                {
                    score = data.Data.gameplay.score;
                    result.Add(temp);
                    temp = new CNoteByNote(score);
                }
            }

            return result.ToArray();
        }

    }

    public class CNoteByNote
    {
        public long maxScore;
        public List<OsuData> Frames = new List<OsuData>();

        public CNoteByNote(long Score) => maxScore = Score;
    }
}
