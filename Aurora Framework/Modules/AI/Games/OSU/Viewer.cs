using Aurora_Framework.Modules.AI.Games.OSU.Data;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Output = SharpDX.DXGI.Output;

namespace Aurora_Framework.Modules.AI.Games.OSU
{
    public class Viewer
    {
        private ImageFilter filter;
        private Screenshot screenshot;
        private Scaler scaler;

        public Viewer(Size Screen, Size View)
        {
            filter = new ImageFilter(View);
            screenshot = new Screenshot(Screen);
            scaler = new Scaler(Screen, View);
        }

        public bool Take(out Bitmap Bitmap)
        {
            if (screenshot.Take(out var screen))
            {
                var view = scaler.Scale(screen);
                Bitmap = filter.FilterV1(view);
                return true;
            }

            Bitmap = null;
            return false;
        }

    }

    public class Screenshot
    {
        SharpDX.Direct3D11.Device device;
        Texture2DDescription textureDesc;
        Output1 output1;
        int height;

        private Bitmap Screen;
        private Size Size;
        public Screenshot(Size Size)
        {
            Screen = new Bitmap(Size.Width, Size.Height, PixelFormat.Format32bppArgb);
            this.Size = Size;

            var factory = new Factory1();
            var adapter = factory.GetAdapter1(0);
            Console.WriteLine(adapter.Description1.Description);
            device = new SharpDX.Direct3D11.Device(adapter);
            Output output = adapter.GetOutput(0);
            Console.WriteLine(output.Description.DeviceName);
            output1 = output.QueryInterface<Output1>();

            int width = Size.Width;
            height = Size.Height;

            textureDesc = new Texture2DDescription
            {
                CpuAccessFlags = CpuAccessFlags.Read,
                BindFlags = BindFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                OptionFlags = ResourceOptionFlags.None,
                MipLevels = 1,
                ArraySize = 1,
                SampleDescription = { Count = 1, Quality = 0 },
                Usage = ResourceUsage.Staging
            };

            screenTexture = new Texture2D(device, textureDesc);
            duplicatedOutput = output1.DuplicateOutput(device);
            Thread.Sleep(20); // захватчику экрана надо время проинициализироваться
        }

        Texture2D screenTexture;
        OutputDuplication duplicatedOutput;
        SharpDX.DXGI.Resource screenResource;
        public bool Take(out Bitmap ScreenShot)
        {
            ScreenShot = Screen;

            if (duplicatedOutput.TryAcquireNextFrame(2, out _, out screenResource) != Result.Ok)
                return false;

            using (Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>())
            {
                device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);
            }

            DataBox mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
            BitmapData fullData = Screen.LockBits(new Rectangle(Point.Empty, Size), ImageLockMode.WriteOnly, Screen.PixelFormat);
            IntPtr sourcePtr = mapSource.DataPointer;
            IntPtr destPtr = fullData.Scan0;
            Utilities.CopyMemory(destPtr, sourcePtr, mapSource.RowPitch * height);
            Screen.UnlockBits(fullData);
            device.ImmediateContext.UnmapSubresource(screenTexture, 0);
            duplicatedOutput.ReleaseFrame();
            return true;
        }
    }

    public class ImageFilter
    {
        float realMax = 255;
        float avg = 1f / 3f;

        public Color[,] Back;
        private Size Size;
        public ImageFilter(Size Size)
        {
            this.Size = Size;
            this.Back = new Color[Size.Width, Size.Height];
            Result = new Bitmap(Size.Width, Size.Height);
            Result2 = new Bitmap(Size.Width, Size.Height);
        }

        Bitmap Result;
        Bitmap Result2;
        public Bitmap Input(Color[,] Input)
        {
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                {
                    var p1 = Back[x, y];
                    var p2 = Input[x, y];
                    Back[x, y] = p2;

                    int r = p2.R - p1.R;
                    int g = p2.G - p1.G;
                    int b = p2.B - p1.B;

                    if (r == 0 && g == 0 && b == 0)
                    {
                        Result.SetPixel(x, y, Color.Black);
                        continue;
                    }

                    int min = r;
                    if (min > g) min = g;
                    if (min > b) min = b;

                    r -= min;
                    g -= min;
                    b -= min;

                    if (r == g && g == b && b == 0) continue;

                    int max = r;
                    if (max < g) max = g;
                    if (max < b) max = b;

                    float dMdR = realMax / max * avg;
                    r = (int)(r * dMdR);
                    g = (int)(g * dMdR);
                    b = (int)(b * dMdR);

                    int value = r + g + b;

                    var color = Color.FromArgb(value, value, value);
                    Result.SetPixel(x, y, color);
                }

            return this.Result;
        }

        public Bitmap FilterV1(Color[,] Input)
        {
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                {
                    var p = Input[x, y];

                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    int value = r + g + b;
                    value = value % 256;

                    var color = Color.FromArgb(value, value, value);
                    Result2.SetPixel(x, y, color);
                }

            return this.Result2;
        }


        /*
        private Bitmap Result;
        public Bitmap Input(Bitmap Input)
        {
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                {
                    var p1 = Back[x, y];
                    var p2 = Input.GetPixel(x, y);
                    Back[x, y] = p2;


                    int r = p1.R - p2.R;
                    int g = p1.G - p2.G;
                    int b = p1.B - p2.B;

                    if (r == 0 && g == 0 && b == 0)
                    {
                        Result.SetPixel(x,y, Color.Black);
                        continue;
                    }

                    int min = r;
                    if (min > g) min = g;
                    if (min > b) min = b;

                    r = r - min;
                    g = g - min;
                    b = b - min;

                    int max = r;
                    if (max < g) max = g;
                    if (max < b) max = b;

                    float dMdR;
                    if (max != 0)
                    {
                        float realMax = 255;
                        dMdR = realMax / Math.Abs(max);
                    }
                    else dMdR = 1;                    

                    r = (int)(r * dMdR);
                    g = (int)(g * dMdR);
                    b = (int)(b * dMdR);

                    var v2 = FilterV2(r, g, b);

                    Color c = Color.FromArgb(v2, v2, v2);
                    Result.SetPixel(x, y, c);
                }

            return Result;
        }

        */
        private int FilterV2(int R, int G, int B)
        {
            int avg = R + G + B;
            avg %= 256;
            return avg;
        }
    }

    public class Scaler
    {
        public Size Input;
        public Size Output;

        private int xScaleOffset;
        private int yScaleOffset;

        public Scaler(Size Input, Size Output)
        {
            this.Input = Input;
            this.Output = Output;

            xScaleOffset = Input.Width / Output.Width;
            yScaleOffset = Input.Height / Output.Height;

            Temp = new Color[Output.Width, Output.Height];
        }

        private Color[,] Temp;
        public Color[,] Scale(Bitmap Image)
        {
            for (int y = 0; y < Output.Height; y++)
                for (int x = 0; x < Output.Width; x++)
                    Temp[x, y] = Image.GetPixel(x * xScaleOffset, y * yScaleOffset);

            return Temp;
        }
    }
}
