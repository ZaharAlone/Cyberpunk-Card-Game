using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace CyberNet
{
    [Serializable]
    public struct ActionCardViewConfig
    {
        [FormerlySerializedAs("NameLoc")]
        [JsonProperty("action_loc_header")]
        public string HeaderLoc;
        [JsonProperty("action_loc_descr")]
        public string DescrLoc;
    }
}