using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CyberNet.Core.Dialog
{
    [CreateAssetMenu(fileName = "DialogConfigSO", menuName = "Scriptable Object/Dialog Config")]
    public class DialogConfigSO : SerializedScriptableObject
    {
        public Dictionary<string, Sprite> DialogAvatars = new Dictionary<string, Sprite>();
        public TextAsset DialogConfigJson;
        public TextAsset CharacterDialogConfigJson;

#if UNITY_EDITOR
        [InfoBox("Найдет ВСЕ спрайты в папке \"Assets/Art/2D/DialogueCharacter\" и перезапишет поле \"Avatars\"!")]
        [ReadOnly]
        [SerializeField]
        private string folderPath = "Assets/Art/2D/DialogueCharacter";
        [Button("Заполнить Словарь")]
        public void FindAllAvatar()
        {
            DialogAvatars.Clear();

            var instances = GetAllInstances<Sprite>(new[] { folderPath });

            foreach (var instance in instances)
            {
                if (DialogAvatars.ContainsKey(instance.name))
                {
                    Debug.LogError($"Аватар с именем {instance.name} присутствует в папке {folderPath} дважды!");
                    DialogAvatars.Clear();
                    break;
                }

                DialogAvatars.Add(instance.name, instance);
            }

            AssetDatabase.SaveAssetIfDirty(this);
        }

        private static T[] GetAllInstances<T>(string[] folders) where T : Object
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, folders);
            T[] a = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }

            return a;
        }
#endif
    }
}