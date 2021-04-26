using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace ReworkedDCC
{
    class SaveSystem
    {
        public static void Save(string path)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            Data data = new Data
            {
                CacheDirectory = Form1.cacheDirectory,
                RunAtStart = Form1.runAtStart
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);
        }

        public static void InitSave(string path)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            Data data = new Data
            {
                CacheDirectory = null,
                RunAtStart = true
            };

            string json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(path, json);
        }

        public static void LoadSave(string path)
        {
            string json = File.ReadAllText(path);

            var data = JsonSerializer.Deserialize<Data>(json);

            Form1.cacheDirectory = data.CacheDirectory;
            Form1.runAtStart = data.RunAtStart;
        }

        public class Data
        {
            public string CacheDirectory { get; set; }
            public bool RunAtStart { get; set; }
        }
    }
}
