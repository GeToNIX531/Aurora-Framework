using System;
using System.Drawing;
using System.Linq;

namespace Aurora_Framework.Modules.AI.Games.OSU.Data
{
    public class DB
    {
        private readonly Input Input;
        private readonly Output Output;

        private int deltaScore;

        private Size ScreenSize;
        public DB(Size ScreenSize, Input input, Output output)
        {
            this.ScreenSize = ScreenSize;
            this.Input = input;
            this.Output = output;
        }

        public double[] InputValues => Input.Get(ScreenSize);
        public double[] OutputValues => Output.Get();

        public int X => Output.x;
        public int Y => Output.y;
    }

    public class Input
    {
        private readonly float[] Screen;
        private readonly Point CursorPosition;

        public Input(float[] Screen, Point CursorPosition)
        {
            this.Screen = Screen;
            this.CursorPosition = CursorPosition;
        }

        public double[] Get(Size ScreenSize)
        {
            double[] result = new double[Screen.Count()];

            int i;
            for (i = 0; i < Screen.Length; i++)
                result[i] = Screen[i];

            //result[i] = (double)CursorPosition.X / ScreenSize.Width;
            //result[i + 1] = (double)CursorPosition.Y / ScreenSize.Height;

            return result;
        }
    }

    public class Output
    {
        public readonly int x;
        public readonly int y;

        private readonly bool aClick;
        private readonly bool bClick;

        public Output(Point Position, bool aClick, bool bClick)
        {
            x = Position.X;
            y = Position.Y;

            this.aClick = aClick;
            this.bClick = bClick;
        }

        public double[] Get(double xMaxMove = 1920d, double yMaxMove = 1080d)
        {
            double[] result = new double[4];
            result[0] = aClick ? 1f : 0f;
            result[1] = bClick ? 1f : 0f;

            double x = this.x / xMaxMove;
            double y = this.y / yMaxMove;
            
            result[2] = x;
            result[3] = y;

            return result;
        }
    }
}
