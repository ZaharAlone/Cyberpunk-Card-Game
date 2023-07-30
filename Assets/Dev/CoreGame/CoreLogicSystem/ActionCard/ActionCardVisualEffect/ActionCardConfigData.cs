using System.Collections.Generic;
using UnityEngine.Serialization;
namespace CyberNet.Core.ActionCard
{
    public struct ActionCardConfigData
    {
        public ActionCardConfig ActionCardConfig;
        public Dictionary<string, ActionCardViewConfig> ActionCardViewConfig;
    }
}