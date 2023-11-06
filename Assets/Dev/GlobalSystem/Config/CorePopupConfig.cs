using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace CyberNet
{
    [Serializable]
    public struct CorePopupConfig
    {
        [JsonProperty("is_popup")]
        public bool IsPoput;
        [JsonProperty("type_popup_position")]
        public CorePopupTypePosition CorePopupTypePosition;
        [JsonProperty("header_loc")]
        public string HeaderLoc;
        [JsonProperty("descr_loc")]
        public string DescrLoc;
        [JsonProperty("artistic_descr_loc")]
        public string ArtisticDescrLoc;
    }

    [Serializable]
    public enum CorePopupTypePosition
    {
        LeftRight,
        Up,
        Down,
    }
}