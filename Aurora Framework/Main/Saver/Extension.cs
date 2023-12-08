using Newtonsoft.Json;
using System;
using System.IO;

namespace Data.Saver
{
    public static class Extension
    {
        public static bool Save<T>(this T Object, string Path, out Exception Error)
        {
            Error = null;

            try
            {
                string text = JsonConvert.SerializeObject(Object);
                File.WriteAllText(Path, text);
                return true;
            }
            catch (Exception e)
            {
                Error = e;
                return false;
            }
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
