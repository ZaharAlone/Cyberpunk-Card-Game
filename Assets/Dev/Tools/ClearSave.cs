using UnityEngine;
using UnityEditor;
using System.IO;

namespace CyberNet.Tools
{
#if UNITY_EDITOR
    public class ClearSave : EditorWindow
    {
        [MenuItem("Tools/Clear Save", priority = -1000)]
        public static void Init()
        {
            var directory = new DirectoryInfo(Application.persistentDataPath);
            directory.Delete(true);
        }
    }
#endif
}