using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Global.Settings
{
    public class LanguageDropdownMono : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown _dropdown;
        
        public void OnEnable()
        {
            var currentLanguage = LocalizationManager.CurrentLanguage;
            if (LocalizationManager.Sources.Count==0) LocalizationManager.UpdateSources();
            var languages = LocalizationManager.GetAllLanguages();
 
            // Fill the dropdown elements
            _dropdown.ClearOptions();
            _dropdown.AddOptions( languages );
 
            _dropdown.value = languages.IndexOf( currentLanguage );
            _dropdown.onValueChanged.RemoveListener( OnValueChanged );
            _dropdown.onValueChanged.AddListener( OnValueChanged );
        }
 
       
        public void OnValueChanged(int index)
        {
            if (index<0)
            {
                index = 0;
                _dropdown.value = index;
            }
 
            LocalizationManager.CurrentLanguage = _dropdown.options[index].text;
        }   
    }
}