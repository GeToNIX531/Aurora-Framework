using Data.Saver;
using System;
using System.IO;
using System.Linq;

namespace Modules.Wiki
{
    public class Data
    {
        public string Path { get; private set; }
        public string Title { get; private set; }
        public string Text { get; private set; }

        public Data(string Title, string Text)
        {
            this.Title = Title;
            this.Text = Text;
            SetPath();
        }

        public void SetPath(params string[] Path)
        {
            if (Path == null) Path = new string[0];

            var path = Path.Prepend("Wiki").ToArray();
            string directory = string.Empty;

            for (int i = 0; i < path.Length; i++)
            {
                directory += string.Format("{0}/", path[i]);
            }

            this.Path = directory;
        }

        public bool Save(out Exception Error)
        {
            if (Directory.Exists(Path) == false)
                Directory.CreateDirectory(Path);

            return this.Save(string.Format("{0}{1}.sd", Path, Title), out Error);
        }
    }
}
