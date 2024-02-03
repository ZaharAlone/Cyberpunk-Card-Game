using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Global.Settings
{
    /// <summary>
    /// Временный код смены языка
    /// </summary>
    public class SetLanguageButton : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private TextMeshProUGUI _languageText;
        [SerializeField]
        private TextMeshProUGUI _languageText_select;

        private List<string> _allLanguage;
        private int _currentIndex;
        private int _maxLanguage;
        
        public void OnEnable()
        {
            var currentLanguage = LocalizationManager.CurrentLanguage;
            if (LocalizationManager.Sources.Count==0)
                LocalizationManager.UpdateSources();
            _allLanguage = LocalizationManager.GetAllLanguages();

            _maxLanguage = _allLanguage.Count;
            _currentIndex= _allLanguage.IndexOf( currentLanguage );
            _languageText.text = currentLanguage;
            _languageText_select.text = currentLanguage;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _currentIndex++;
            if (_currentIndex >= _maxLanguage)
                _currentIndex = 0;

            UpdateCurrentLanguage();
        }

        private void UpdateCurrentLanguage()
        {
            _languageText.text = _allLanguage[_currentIndex];
            _languageText_select.text = _allLanguage[_currentIndex];
            LocalizationManager.CurrentLanguage = _allLanguage[_currentIndex];
        }
    }
}