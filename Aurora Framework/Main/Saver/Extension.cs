using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Data.Saver
{
    public static class Extension
    {
        public static bool Save<T>(this T Object, string Path, out Exception Error)
        {
            Error = null;

            var pat = Path.Split('/', '\\');

            string dir = string.Empty;
            for (int i = 0; i < pat.Length - 1; i++)
            {
                string temp = dir + pat[i] + '/';
                if (Directory.Exists(temp) == false)
                    Directory.CreateDirectory(temp);
                dir = temp;
            }

            string file = dir + pat[pat.Length - 1];
            if (File.Exists(file) == false)
                File.CreateText(file).Close();

            string text = JsonConvert.SerializeObject(Object);
            File.WriteAllText(file, text);
            return true;
        }

        public static bool Load<T>(this string Path, out T Value, out Exception Error)
        {
            Value = default;
            Error = default;
            if (File.Exists(Path) == false) return false;

            try
            {
                string text = File.ReadAllText(Path);
                Value = JsonConvert.DeserializeObject<T>(text);
                return true;
            }
            catch (Exception e)
            {
                Error = e;
                return false;
            }
        }

        public static bool Load<T>(this string[] Paths, out T[] Values, out Exception Error)
        {
            Values = default;
            Error = default;

            if (Paths == default) return false;

            try
            {
                int count = Values.Length;
                T[] result = new T[count];
                for (int i = 0; i < count; i++)
                {
                    string text = File.ReadAllText(Paths[i]);
                    result[i] = JsonConvert.DeserializeObject<T>(text);
                }

                Values = result;
                return true;
            }
            catch (Exception e)
            {
                Error = e;
                return false;
            }
        }
        public static bool Save<T>(this T[] Values, string[] Paths, out Exception Error)
        {
            Error = default;

            int count = Values.Length;
            for (int i = 0; i < count; i++)
            {
                if (Values[i].Save(Paths[i], out Error) == false)
                    return false;
            }

            return true;
        }

        public static bool Save(this ISave Object, out Exception Error) => Object.GetObject().Save(Object.GetSavePath(), out Error);
    }
}
