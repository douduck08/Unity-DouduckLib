using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace DouduckLib.Save
{
    public interface ISaveData
    {
        void Resolve();
    }

    public static class SaveFileHelper
    {
        static string _defaultSaveDir;

        public static string DefaultSaveDir
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultSaveDir))
                {
                    try
                    {
                        var rootDir = Directory.GetParent(Application.streamingAssetsPath)?.Parent?.FullName ?? Application.persistentDataPath;
                        _defaultSaveDir = Path.GetFullPath(Path.Combine(rootDir, "Save/"));
                    }
                    catch (Exception)
                    {
                        _defaultSaveDir = Path.GetFullPath(Path.Combine(Application.persistentDataPath, "Save/"));
                    }
                }
                return _defaultSaveDir;
            }
        }

        public static void Save<T>(T data, string fileName)
        {
            Directory.CreateDirectory(DefaultSaveDir);
            string jsonData = JsonConvert.SerializeObject(data);
            var filePath = Path.Combine(DefaultSaveDir, fileName);
            using (var writer = new StreamWriter(filePath, false))
            {
                writer.WriteLine(jsonData);
            }
        }

        public static bool Load<T>(string fileName, out T data)
        {
            var filePath = Path.Combine(DefaultSaveDir, fileName);
            if (!File.Exists(filePath))
            {
                data = default;
                return false;
            }

            string str;
            using (var reader = new StreamReader(filePath))
            {
                str = reader.ReadToEnd();
            }

            data = JsonConvert.DeserializeObject<T>(str);
            if (data is ISaveData saveData)
            {
                saveData.Resolve();
            }
            return true;
        }
    }
}
