using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Device Display Settings", menuName = "Scriptable Object/Setting/Device Display Settings", order = 1)]
public class DeviceDisplaySettings : ScriptableObject
{
    public List<InputContext> customContextIcons = new List<InputContext>();
}

[System.Serializable]
public struct InputContext
{
    public string InputString;
    public Sprite InputIcons;
}