using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Avatars", menuName = "Scriptable Object/Board Game/Avatars")]
public class AvatarListSO : SerializedScriptableObject
{
    public Dictionary<string, Sprite> Avatar = new Dictionary<string, Sprite>();

#if UNITY_EDITOR
    [InfoBox("Найдет ВСЕ Иконки в папке \"Assets/Art/2D/Avatars\" и перезапишет поле \"Avatar\"!")]
    [SerializeField]
    private string folderPath = "Assets/Art/2D/Avatars";
    [Button("Заполнить Словарь")]
    public void FindAllEquipViewSO()
    {
        Avatar.Clear();

        var instances = GetAllInstances<Sprite>(new[] { folderPath });

        foreach (var instance in instances)
        {
            if (Avatar.ContainsKey(instance.name))
            {
                Debug.LogError($"Иконка с именем {instance.name} присутствует в папке {folderPath} дважды!");
                Avatar.Clear();
                break;
            }

            Avatar.Add(instance.name, instance);
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