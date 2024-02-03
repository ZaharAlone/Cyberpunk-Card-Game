using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace CyberNet.Core.UI.CorePopup
{
    [Serializable]
    public struct CorePopupData
    {
        public Dictionary<string, CorePopupConfig> CorePopupConfig;
        public Dictionary<string, CorePopupTaskConfig> CorePopupTaskConfig;
    }
}