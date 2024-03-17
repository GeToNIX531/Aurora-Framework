using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Aurora_Framework.Modules.Pythone
{
    public class Script
    {
        public const string ScriptsDirectory = "pythone";
        public string SubDirectory = "Scrpts";

        private string[] lines;
        public Script(int LineSize)
        {
            lines = new string[LineSize];
            indexer = 0;
        }

        private int indexer;

        public void Add(params string[] Commands)
        {
            foreach (var cmd in Commands)
            {
                lines[indexer] = cmd;
                indexer++;
            }
        }

        private string path;
        public string Save(string FileName) 
        {
            if (FileName.EndsWith(".py") == false)
                FileName += ".py";

            string dir = $"{Directory.GetCurrentDirectory()}\\{ScriptsDirectory}\\{SubDirectory}";
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);

            path = $"{dir}\\{FileName}";

            using (var fs = File.Create(path))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    foreach (var line in lines)
                        if (string.IsNullOrEmpty(line) == false)
                            sw.WriteLine(line);

                    sw.Close();
                }

                fs.Close();
            }

            return path;
        }

        public void Run(Logger Logger)
        {
            var client = new Client(Logger);
            client.OpenScript(path);
            client.Run();
        }
    }
}
