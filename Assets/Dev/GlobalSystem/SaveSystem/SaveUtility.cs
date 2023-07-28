using UnityEngine;
using System.IO;

namespace CyberNet.SaveSystem
{
    public static class SaveUtility
    {
        public static bool CheckFile(string directory, string filename)
        {
            if (!Directory.Exists(Application.persistentDataPath + directory))
                Directory.CreateDirectory(Application.persistentDataPath + directory);

            var fullPath = Application.persistentDataPath + directory + filename;
            return File.Exists(fullPath);
        }

        public static void Save(object save, string directory, string filename)
        {
            var dir = Application.persistentDataPath + directory;

            var json = JsonUtility.ToJson(save);
            File.WriteAllText(dir + filename, json);
        }

        public static string Load(string directory, string filename)
        {
            var fullPath = Application.persistentDataPath + directory + filename;
            var json = "";
            if (File.Exists(fullPath))
            {
                json = File.ReadAllText(fullPath);
            }
            else
                Debug.Log("Save file does not exist");
            return json;
        }
    }
}