using System.Drawing;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.AI.Games.OSU.Data
{
    public class FrameData
    {
        public OsuPPCounter.Data OsuPPCounter;
        public Point CursorPosition;
        public Keys Buttons;
        public bool Status;
        public long Frame;


        public bool ButtonAPress;
        public bool ButtonBPress;

        public FrameData(long Frame, Point CursorPosition, bool APress, bool BPress)
        {
            this.Frame = Frame;
            this.CursorPosition = CursorPosition;
            this.ButtonAPress = APress;
            this.ButtonBPress = BPress;
        }
    }

    public class FrameImage
    {
        public Image Values;
        public Size Size;

        public FrameImage(Size Size, Image Value)
        {
            this.Size = Size;
            this.Values = Value;
        }
    }
}
