using System;
using Newtonsoft.Json;
using System.IO;

namespace Aurora_Framework.Main.Buffers
{
    public class Cache<T>
    {
        private T[] Array;

        public int Size { get; private set; }
        public int index { get; private set; }

        public Cache(int Size = 100)
        {
            this.Size = Size;
            Array = new T[Size];

            if (Size == 0)
                throw new Exception($"{nameof(Cache<T>)} размер массива равен 0");
        }

        private string directory;
        public void SetSave(string Directory)
        {
            directory = Directory;
        }

        public bool Add(T Element)
        {
            if (index == Size - 1)
            {
                Save();
                Clear();
                return false;
            }
            else
            {
                Array[index] = Element;
                index++;
                count++;

                return index == Size - 1;
            }
        }

        public T Get(int Index)
        {
            int index = Index % Size;

            return Array[index];
        }
        public T Get(Random RND)
        {
            int index = RND.Next(0, this.index);
            return Array[index];
        }

        long count;
        public void Save()
        {
            string content = JsonConvert.SerializeObject(Array);

            if (Directory.Exists(directory) == false)
                Directory.CreateDirectory(directory);
            string path = $"{directory}/{count}-{index}.json";

            File.WriteAllText(path, content);
        }

        public void Clear()
        {
            for (index = 0; index < Size; index++)
                Array[index] = default;
            index = 0;
        }

        public void Optimized()
        {
            int count = index;
            var array = new T[count];
            for (int i = 0; i < count; i++)
                array[i] = Array[i];

            Array = array;
            Size = count;
        }

        public void AddSize(int Value = 100)
        {
            int count = index;
            int size = Size + Value;

            T[] array = new T[size];
            for (int i = 0; i < count; i++)
                array[i] = Array[i];

            Array = array;
            Size = size;
        }
    }
}
