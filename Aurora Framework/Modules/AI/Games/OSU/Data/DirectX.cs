using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

public class DirectX : IDisposable
{
    SharpDX.Direct3D11.Device device;
    Texture2DDescription textureDesc;
    Output1 output1;

    Size Size;
    int height;
    public DirectX(Size Size)
    {
        this.Size = Size;
        this.height = Size.Height;
        full = new Bitmap(Size.Width, Size.Height);

        var factory = new Factory1();
        var adapter = factory.GetAdapter1(0);
        device = new SharpDX.Direct3D11.Device(adapter);
        Output output = adapter.GetOutput(0);
        output1 = output.QueryInterface<Output1>();

        textureDesc = new Texture2DDescription
        {
            CpuAccessFlags = CpuAccessFlags.Read,
            BindFlags = BindFlags.None,
            Format = Format.B8G8R8A8_UNorm,
            Width = Size.Width,
            Height = Size.Height,
            OptionFlags = ResourceOptionFlags.None,
            MipLevels = 1,
            ArraySize = 1,
            SampleDescription = { Count = 1, Quality = 0 },
            Usage = ResourceUsage.Staging
        };

        screenTexture = new Texture2D(device, textureDesc);
        duplicatedOutput = output1.DuplicateOutput(device);
        Thread.Sleep(100); // захватчику экрана надо время проинициализироваться
    }
    Texture2D screenTexture;
    OutputDuplication duplicatedOutput;

    SharpDX.DXGI.Resource screenResource;

    private Bitmap full;
    public bool TakeScreenshot(out Bitmap bitmap)
    {
        bitmap = full;

        if (duplicatedOutput.TryAcquireNextFrame(1000, out _, out screenResource) != Result.Ok)
            return false;

        using (Texture2D screenTexture2D = screenResource.QueryInterface<Texture2D>())
        {
            device.ImmediateContext.CopyResource(screenTexture2D, screenTexture);
        }

        DataBox mapSource = device.ImmediateContext.MapSubresource(screenTexture, 0, MapMode.Read, SharpDX.Direct3D11.MapFlags.None);
        BitmapData fullData = full.LockBits(new Rectangle(Point.Empty, Size), ImageLockMode.WriteOnly, full.PixelFormat);
        IntPtr sourcePtr = mapSource.DataPointer;
        IntPtr destPtr = fullData.Scan0;
        Utilities.CopyMemory(destPtr, sourcePtr, mapSource.RowPitch * height);
        full.UnlockBits(fullData);
        device.ImmediateContext.UnmapSubresource(screenTexture, 0);
        duplicatedOutput.ReleaseFrame();
        return true;
    }

    public void Dispose()
    {
        device?.Dispose();
        output1?.Dispose();
        duplicatedOutput?.Dispose();
        screenResource?.Dispose();
    }
}