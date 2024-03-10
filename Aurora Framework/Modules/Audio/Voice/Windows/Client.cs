using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.AudioFormat;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace Aurora_Framework.Modules.Audio.Voice.Windows
{
    public class Client : IDisposable
    {
        SpeechSynthesizer synthesizer;
        public Client()
        {
            synthesizer = new SpeechSynthesizer();
        }

        public void Dispose()
        {
            synthesizer.Dispose();
            synthesizer = null;
        }

        public MemoryStream Speak(string Text)
        {
            using (MemoryStream streamAudio = new MemoryStream())
            {
                using (var m_SoundPlayer = new System.Media.SoundPlayer())
                {
                    synthesizer.SetOutputToWaveStream(streamAudio);

                    synthesizer.Speak(Text);
                    streamAudio.Position = 0;
                    m_SoundPlayer.Stream = streamAudio;
                    m_SoundPlayer.Play();

                    synthesizer.SetOutputToNull();
                }
                return streamAudio;
            }
        }
    }
}
