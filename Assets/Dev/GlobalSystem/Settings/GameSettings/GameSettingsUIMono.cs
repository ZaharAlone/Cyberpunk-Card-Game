using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Global.Settings
{
    public class GameSettingsUIMono : MonoBehaviour
    {
        [SerializeField]
        private Toggle _toglleDistrictPopup;

        public void SetView(GameSettingsConfig config)
        {
            
        }
        
        public void SetShowDistrictPopup(bool value)
        {
            SettingsAction.SetShowDistrictPopup?.Invoke(value);
        }

        public void SetViewDistrict(bool value)
        {
            _toglleDistrictPopup.isOn = value;
        }
    }
}