using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace Aurora_Framework.Modules.Pythone
{
    public class Client
    {
        private const string pythoneZip = "Pythone.zip";
        private const string pythoneEngine = "PythoneEngine";

        public Logger Logger;

        private string pythonePath;
        public Client()
        {
            Init();
        }

        public Client(Logger Logger)
        {
            this.Logger = Logger;
            Init();
        }

        public void Init()
        {
            pythonePath = $"{Directory.GetCurrentDirectory()}\\{pythoneEngine}\\Pythone\\App\\Python\\python.exe";

            #region Распаковка Pythone
            if (Directory.Exists(pythoneEngine) == false)
            {
                Logger?.Send("Распаковка Python");
                Directory.CreateDirectory(pythoneEngine);


                var ps = new Process()
                {
                    StartInfo = new ProcessStartInfo("powershell")
                    {
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };

                ps.Start();
                var reader = ps.StandardInput;
                reader.WriteLine($"Expand-Archive -Path \"{Directory.GetCurrentDirectory()}\\{pythoneZip}\"  -DestinationPath \"{Directory.GetCurrentDirectory()}/{pythoneEngine}\" -Force");
                reader.Flush();
                reader.Close();


                ps.WaitForExit();
                ps.Close();

                Logger?.Send("Распаковка Python прошла успешно");

            }
            else Logger?.Send("Распаковка Python не требуется");

            #endregion
        }

        Process process;
        public void OpenScript(string FullPath)
        {
            if (File.Exists(FullPath) == false)
                Logger?.Send("Скрипт не найден");


            Logger?.Send("Подготовка к запуска скрипта");
            Logger?.Send(FullPath);
            process = new Process()
            {
                StartInfo = new ProcessStartInfo(pythonePath)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = $"\"{FullPath}\"",
                    CreateNoWindow = true
                },

            };

            process.OutputDataReceived += Output;
            process.ErrorDataReceived += ErrorOutput;
            Logger?.Send("Подготовка к запуска скрипта - Успешно");

        }

        private void Output(object sender, DataReceivedEventArgs e)
        {
            if (process.HasExited)
                Logger?.Send("Скрипт завершил выполнение");

            if (string.IsNullOrEmpty(e.Data))
                return;

            Logger?.Send(e.Data);
        }

        private void ErrorOutput(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
                return;

            Logger?.SendError(e.Data);
        }

        public void Run()
        {
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            Logger?.Send("Скрипт запущен");

        }
    }
}
