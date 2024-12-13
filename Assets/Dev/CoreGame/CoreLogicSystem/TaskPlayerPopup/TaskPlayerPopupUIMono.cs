using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Core.UI.TaskPlayerPopup
{
    public class TaskPlayerPopupUIMono : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private GameObject _panel;
        [SerializeField]
        [Required]
        private RectTransform _rect;
        
        [Header("Localize")]
        [SerializeField]
        [Required]
        private Localize _headerLocalize;
        [SerializeField]
        [Required]
        private LocalizationParamsManager _headerLocalizeParams;
        [SerializeField]
        [Required]
        private Localize _descrLocalize;
        [SerializeField]
        [Required]
        private LocalizationParamsManager _descrLocalizeParams;
        
        [Header("Text")]
        [SerializeField]
        [Required]
        private TextMeshProUGUI _headerText;
        [SerializeField]
        [Required]
        private TextMeshProUGUI _descrText;
        
        public void OpenWindowSetLocalizeTerm(string header, string descr)
        {
            _headerLocalize.gameObject.SetActive(!string.IsNullOrEmpty(header));
            _descrLocalize.gameObject.SetActive(!string.IsNullOrEmpty(descr));
            
            _headerLocalize.Term = header;
            _descrLocalize.Term = descr;
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
            _panel.SetActive(true);
        }

        public void SerLocalizeParameter(string param)
        {
            _headerLocalizeParams.SetParameterValue("count", param);
            _descrLocalizeParams.SetParameterValue("count", param);
        }
        
        public void OpenWindowSetText(string header, string descr)
        {
            _headerText.gameObject.SetActive(header != "");
            _descrText.gameObject.SetActive(header != "");
            
            _headerText.text = header;
            _descrText.text = descr;
            _panel.SetActive(true);
        }

        public void CloseWindow()
        {
            _panel.SetActive(false);
        }
    }
}