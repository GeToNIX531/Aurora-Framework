using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.AI.Games.OSU.Forms
{
    public partial class Viewer : Form
    {
        Size size;
        Size sizefull;
        Bitmap full;
        Tracker tracker;
        public Viewer()
        {
            tracker = new Tracker();
            tracker.UpdateStatusEvent += TrackerUpdateStatus;
            tracker.Show();

            InitializeComponent();
            size = new Size(80, 60);
            sizefull = new Size(1920, 1080);
            full = new Bitmap(sizefull.Width, sizefull.Height, PixelFormat.Format32bppArgb);
            img = new Color[size.Width, size.Height];
            iFilter = new ImageFilter(size);

            Init();

            xOffset = sizefull.Width / size.Width;
            yOffset = sizefull.Height / size.Height;

           
        }

        ImageFilter iFilter;
        int count = 0;

        bool ready = true;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (ready)
            {
                ready = false;
                Tick();
                ready = true;
            }
        }

        private bool TrackerStatus = false;
        private void TrackerUpdateStatus(bool Status)
        {
            TrackerStatus = Status;
        }

        long frame = 0;
        private void Tick()
        {
            if (TakeScreenshot() == false) return;

            if (TrackerStatus)
            {
                frame++;
                tracker.Tick(frame);
            }
            Render();

            if (TrackerStatus)
            {
                string path = $"Frames/{frame}.png";
                if (File.Exists(path)) File.Delete(path);
                renderImage.Save(path, ImageFormat.Png);
                count += 1;
            }
        }


        Color[,] img;
        int xOffset;
        int yOffset;

        Bitmap renderImage;

        private void Render()
        {
            for (int y = 0; y < size.Height; y++)
                for (int x = 0; x < size.Width; x++)
                    img[x, y] = full.GetPixel(x * xOffset, y * yOffset);

            //var iImage = iFilter.Input(img);
            renderImage = iFilter.FilterV1(img, out float[] aiInput);
            pictureBox1.Image = renderImage;

            /*
            iFilter.Back = back;
            var iImage2 = iFilter.FilterV1(img);
            pictureBox2.Image = iImage2;
            */
        }

        SharpDX.Direct3D11.Device device;
        Texture2DDescription textureDesc;
        Output1 output1;
        int height;
        private void Init()
        {
            var factory = new Factory1();
            var adapter = factory.GetAdapter1(0);
            Console.WriteLine(adapter.Description1.Description);
            device = new SharpDX.Direct3D11.Device(adapter);
            Output output = adapter.GetOutput(0);
            Console.WriteLine(output.Description.DeviceName);
            output1 = output.QueryInterface<Output1>();

            int width = sizefull.Width;
            height = sizefull.Height;

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
        bool TakeScreenshot()
        {
            if (duplicatedOutput.TryAcquireNextFrame(2, out _, out screenResource) != Result.Ok)
                return false;

            using (Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>())
            {
                device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);
            }

            DataBox mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
            BitmapData fullData = full.LockBits(new Rectangle(Point.Empty, sizefull), ImageLockMode.WriteOnly, full.PixelFormat);
            IntPtr sourcePtr = mapSource.DataPointer;
            IntPtr destPtr = fullData.Scan0;
            Utilities.CopyMemory(destPtr, sourcePtr, mapSource.RowPitch * height);
            full.UnlockBits(fullData);
            device.ImmediateContext.UnmapSubresource(screenTexture, 0);
            duplicatedOutput.ReleaseFrame();
            return true;
        }

        Bitmap TryTakeScreenshot()
        {
            SharpDX.DXGI.Resource screenResource = null;
            try
            {
                if (duplicatedOutput.TryAcquireNextFrame(10, out OutputDuplicateFrameInformation duplicateFrameInformation, out screenResource) != Result.Ok)
                    return full;

                using (Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>())
                {
                    device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);
                }

                DataBox mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
                BitmapData fullData = full.LockBits(new Rectangle(Point.Empty, full.Size), ImageLockMode.WriteOnly, full.PixelFormat);
                IntPtr sourcePtr = mapSource.DataPointer;
                IntPtr destPtr = fullData.Scan0;
                Utilities.CopyMemory(destPtr, sourcePtr, mapSource.RowPitch * height);
                full.UnlockBits(fullData);
                device.ImmediateContext.UnmapSubresource(screenTexture, 0);
                duplicatedOutput.ReleaseFrame();
            }
            catch (SharpDXException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                screenResource?.Dispose();
            }
            return full;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Text = $"FPS: {this.count}";
            this.count = 0;
        }
    }

    public class ImageFilter
    {
        float realMax = 256;
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

        public const float fColor = 1 / 256;
        public Bitmap FilterV1(Color[,] Input, out float[] aiInput)
        {
            aiInput = new float[Size.Height * Size.Width];

            int delta = 0;
            for (int y = 0; y < Size.Height; y++)
                for (int x = 0; x < Size.Width; x++)
                {
                    var p = Input[x, y];

                    int r = p.R;
                    int g = p.G;
                    int b = p.B;

                    var value = r + g + b;
                    value += (int)(delta * 0.1f);
                    value = value % 256;

                    delta = value;

                    var color = Color.FromArgb(value, value, value);
                    Result2.SetPixel(x, y, color);

                    aiInput[Size.Height * y + x] = value * fColor;
                }

            return this.Result2;
        }

        private int FilterV2(int R, int G, int B)
        {
            int avg = R + G + B;
            avg %= 256;
            return avg;
        }
    }

}
