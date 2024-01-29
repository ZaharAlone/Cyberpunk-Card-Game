using System;
using Newtonsoft.Json;

namespace CyberNet
{
    [Serializable]
    public struct CorePopupTaskConfig
    {
        [JsonProperty("header_loc")]
        public string HeaderLoc;
        [JsonProperty("descr_loc")]
        public string DescrLoc;
    }
}