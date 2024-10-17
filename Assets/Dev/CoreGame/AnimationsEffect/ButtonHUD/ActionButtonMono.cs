using CyberNet.Core.UI.CorePopup;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.UI.ActionButton
{
    public class ActionButtonMono : MonoBehaviour
    {
        [Header("Action Button")]
        [Required]
        public GameObject ActionButtonGO;
        [Required]
        public CoreActionButtonAnimationsMono CoreActionButtonAnimationsMono;
        [Required]
        public CoreElementInfoPopupButtonMono PopupActionButton;
    }
}