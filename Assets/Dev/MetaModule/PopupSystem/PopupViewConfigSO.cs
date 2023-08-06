using System.Collections.Generic;
using I2.Loc;
using UnityEngine;

namespace CyberNet.Meta
{
    [CreateAssetMenu(fileName = "PopupViewConfig", menuName = "Scriptable Object/Board Game/Popup View Config", order = 0)]
    public class PopupViewConfigSO : ScriptableObject
    {
        [Header("Confim View Popup")]
        [Header("Exit Game")]
        public LocalizedString HeaderPopupConfim;
        public LocalizedString DescrPopupConfim;
        public LocalizedString ConfimButtonPopupConfim;
        public LocalizedString CancelButtonPopupConfim;
        
        [Header("New campaign")]
        public LocalizedString HeaderPopupNewCampaign;
        public LocalizedString DescrNewCampaign;
        public LocalizedString ConfimButtonPopupNewCampaign;
        public LocalizedString CancelButtonPopupNewCampaign;
        
        [Header("Return Menu")]
        public LocalizedString HeaderPopupReturnMenu;
        public LocalizedString DescrReturnMenu;
        public LocalizedString ConfimButtonPopupReturnMenu;
        public LocalizedString CancelButtonPopupReturnMenu;
    }
}