using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CyberNet
{
    [Serializable]
    public struct TowerConfig
    {
        [JsonProperty("tower_name")]
        public string Name;
        [JsonProperty("nameLoc")]
        public string NameLoc;
        [JsonProperty("reward_end_game")]
        public KeyValue RewardEndGame;
        [JsonProperty("reward_control")]
        public List<KeyValue> RewardControl;
        [JsonProperty("reward_full_control")]
        public List<KeyValue> RewardFullControl;
    }
}