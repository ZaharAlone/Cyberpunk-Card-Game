using System;
using System.Collections.Generic;

namespace CyberNet.Core.UI.CorePopup
{
    [Serializable]
    public struct CorePopupData
    {
        public Dictionary<string, CorePopupConfig> PopupConfig;
    }
}