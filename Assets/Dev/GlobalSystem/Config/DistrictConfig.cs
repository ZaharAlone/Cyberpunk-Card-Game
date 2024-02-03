using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CyberNet
{
    [Serializable]
    public struct DistrictConfig
    {
        [JsonProperty("district_name")]
        public string Name;
        [JsonProperty("nameLoc")]
        public string NameLoc;
        [JsonProperty("descrLoc")]
        public string DescrLoc;
    }
}