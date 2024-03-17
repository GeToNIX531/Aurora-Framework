using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aurora_Framework.Modules.Pythone
{
    public class Test : IDisposable
    {
        private const string pythoneZip = "Pythone.zip";
        private const string pythoneEngine = "PythoneEngine";
        private const string pythonePath = @"H:\Aurora Framework\Aurora Framework\bin\Debug\PythoneEngine\Pythone\App\Python";

        private StreamWriter writer;

        public Logger Logger;


        public Test()
        {
            #region Распаковка Pythone

            if (Directory.Exists(pythoneEngine) == false)
            {
                Directory.CreateDirectory(pythoneEngine);


                var ps = new Process()
                {
                    StartInfo = new ProcessStartInfo("powershell")
                    {
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    }
                };

                ps.Start();
                var reader = ps.StandardInput;
                reader.WriteLine($"Expand-Archive -Path \"{Directory.GetCurrentDirectory()}\\{pythoneZip}\"  -DestinationPath \"{Directory.GetCurrentDirectory()}/{pythoneEngine}\" -Force");
                reader.Flush();
                reader.Close();


                ps.WaitForExit();
                ps.Close();
            }

            #endregion

            #region Запуск движка


            EngineInit();
            #endregion
        }

        public void EngineInit()
        {
            process = new Process()
            {
                StartInfo = new ProcessStartInfo(pythonePath + @"\python.exe")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = "\"H:\\Aurora Framework\\Aurora Framework\\bin\\Debug\\python\\pyramide.py\""
                },

            };

            process.OutputDataReceived += Output;
        }

        private void Output(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data) == false)
                Logger?.Send(e.Data);
        }

        public void Run()
        {
            process.Start();
            process.BeginOutputReadLine();
        }

        public const string DefaultPath = @"PythoneEngine\App\Python\python.exe";

        Process process;
        public LoggerHandler LoggerEvent;

        public delegate void LoggerHandler(string Text);

        public void Dispose()
        {
            LoggerEvent = null;
            process?.Dispose();
        }
    }
}
