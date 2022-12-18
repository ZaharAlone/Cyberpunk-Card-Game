using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Device Display Configurator", menuName = "Scriptable Object/Setting/Device Display Configurator", order = 1)]
public class DeviceDisplayConfigurator : ScriptableObject
{
    public List<DeviceSet> ListDeviceSets = new List<DeviceSet>();
}

[System.Serializable]
public struct DeviceSet
{
    public string deviceRawPath;
    public DeviceDisplaySettings deviceDisplaySettings;
}