using System.Collections.Generic;
using CyberNet.Core.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CardsViewConfigSO", menuName = "Scriptable Object/Board Game/Card View Config")]
public class CardsViewConfigSO : SerializedScriptableObject
{
    public Dictionary<string, Sprite> CardsImageDictionary = new Dictionary<string, Sprite>();

    [Header("Element ability card")]
    public Image IconsBaseAbility;
    public TextMeshProUGUI TextBaseAbilityCountItem;
    public Image IconsArrowConditionAbility;
    public TextAbilityLocalizeMono TextBaseAbility;
    public GameObject ItemIconsCounterCard;
    public GameObject IconsArrowChooseAbility;
    public Image IconsArenaAbility;
    public Image IconsMapAbility;
    
#if UNITY_EDITOR
    [InfoBox("Найдет ВСЕ Иконки в папке \"Assets/Art/2D/Cards\" и перезапишет поле \"Cards\"!")]
    [SerializeField]
    private string folderPath = "Assets/Art/2D/Cards";
    [Button("Заполнить Словарь")]
    public void FindAllImage()
    {
        CardsImageDictionary.Clear();

        var instances = GetAllInstances<Sprite>(new[] { folderPath });

        foreach (var instance in instances)
        {
            if (CardsImageDictionary.ContainsKey(instance.name))
            {
                Debug.LogError($"Иконка с именем {instance.name} присутствует в папке {folderPath} дважды!");
                CardsImageDictionary.Clear();
                break;
            }

            CardsImageDictionary.Add(instance.name, instance);
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