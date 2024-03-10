using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.AI.Games.OSU.Data
{
    public class ImageFilter
    {
        private const int realMax = 256;
        private const float avg = 1f / 3f;
        private const float fColor = 1 / 256f;

        private int W;
        private int H;
        public ImageFilter(Size Size)
        {
            this.W = Size.Width;
            this.H = Size.Height;
        }

        public int[,] FilterV1(Color[,] Input)
        {
            int[,] result = new int[W, H];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    var p = Input[x, y];
                    int r = p.R;
                    int g = p.G;
                    int b = p.G;

                    int value = r + g + b;
                    value = value % 256;
                    result[x, y] = value;
                }

            return result;
        }

        public float[,] FilterV1(Color[,] Input, Bitmap Output)
        {
            float[,] result = new float[W, H];
            for (int y = 0; y < H; y++)
                for (int x = 0; x < W; x++)
                {
                    var p = Input[x, y];
                    int r = p.R;
                    int g = p.G;
                    int b = p.G;

                    int value = r + g + b;
                    value = value % 256;

                    float res = value * fColor;
                    result[x, y] = res;

                    Output.SetPixel(x, y, Color.FromArgb(value, value, value));
                }

            return result;
        }
    }
}
