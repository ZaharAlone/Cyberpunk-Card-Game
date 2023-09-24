using System.Collections.Generic;
using UnityEngine.Serialization;
namespace CyberNet.Core.AbilityCard
{
    public struct ActionCardConfigData
    {
        public ActionCardConfig ActionCardConfig;
        public Dictionary<string, ActionCardViewConfig> ActionCardViewConfig;
    }
}