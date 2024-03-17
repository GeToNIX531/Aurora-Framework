using System.Drawing;

namespace Aurora_Framework.Modules
{
    public class Logger
    {
        private Color MsgColor = Color.Green;
        private string Name;
        public string Format = "[{0}] {1}";

        public event OutputDelegate OutputEvent;

        public Logger(Color Color)
        {
            this.MsgColor = Color;
        }

        public void Send(string Msg)
        {
            string text;
            if (string.IsNullOrEmpty(Name)) text = Msg;
            else text = string.Format(Format, Name, Msg);
            OutputEvent.Invoke(text, MsgColor);
        }

        public void SendError(string Msg)
        {
            string text;
            if (string.IsNullOrEmpty(Name)) text = Msg;
            else text = string.Format(Format, Name, Msg);
            OutputEvent.Invoke(text, Color.Red);
        }

        public void SetName(string Name) => this.Name = Name;

        public delegate void OutputDelegate(string Msg, Color Color);
    }
}
