using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Global.Settings
{
    public class GameSettingsUIMono : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Toggle _districtPopupToggle;

        [Required]
        [SerializeField]
        private Toggle _confirmEndTurnToggle;
        
        public void SetView(GameSettingsConfig config)
        {
            _districtPopupToggle.isOn = config.IsShowDistrickPopup;
            _confirmEndTurnToggle.isOn = config.IsShowWarningPopupEndTurn;
        }
        
        public void SetShowDistrictPopup(bool value)
        {
            SettingsAction.SetShowDistrictPopup?.Invoke(value);
        }
        
        public void SetConfirmEndTurnPopup(bool value)
        {
            SettingsAction.SetShowConfirmEndTurnPopup?.Invoke(value);
        }
    }
}