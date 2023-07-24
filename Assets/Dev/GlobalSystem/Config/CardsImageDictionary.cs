using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CardsImage", menuName = "Scriptable Object/Board Game/Card Image")]
public class CardsImageDictionary : SerializedScriptableObject
{
    public Dictionary<string, Sprite> Cards = new Dictionary<string, Sprite>();

#if UNITY_EDITOR
    [InfoBox("Найдет ВСЕ Иконки в папке \"Assets/Art/2D/Cards\" и перезапишет поле \"Cards\"!")]
    [SerializeField]
    private string folderPath = "Assets/Art/2D/Cards";
    [Button("Заполнить Словарь")]
    public void FindAllImage()
    {
        Cards.Clear();

        var instances = GetAllInstances<Sprite>(new[] { folderPath });

        foreach (var instance in instances)
        {
            if (Cards.ContainsKey(instance.name))
            {
                Debug.LogError($"Иконка с именем {instance.name} присутствует в папке {folderPath} дважды!");
                Cards.Clear();
                break;
            }

            Cards.Add(instance.name, instance);
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