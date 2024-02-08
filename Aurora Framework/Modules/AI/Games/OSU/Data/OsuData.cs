using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.AI.Games.OSU.Data
{
    public class OsuData
    {
        public float[,] Screen;
        public Point CursorPosition;
        public long Frame;

        public bool aDown;
        public bool bDown;

        public OsuPPCounter.Data Data;

        public OsuData(float[,] Screen, Point Cursor, long Frame, bool aDown, bool bDown, OsuPPCounter.Data Data)
        {
            this.Screen = Screen;
            this.CursorPosition = Cursor;
            this.Frame = Frame;

            this.aDown = aDown;
            this.bDown = bDown;
            this.Data = Data;
        }
    }
}
