using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.UI.TaskPlayerPopup
{
    public class TaskPlayerPopupUIMono : MonoBehaviour
    {
        [FormerlySerializedAs("Panel")]
        [SerializeField]
        private GameObject _panel;
        
        [Header("Localize")]
        [SerializeField]
        private Localize _headerLocalize;
        [SerializeField]
        private Localize _descrLocalize;
        
        [Header("Text")]
        [SerializeField]
        private TextMeshProUGUI _headerText;
        [SerializeField]
        private TextMeshProUGUI _descrText;
        
        public void OpenWindowSetLocalizeTerm(string header, string descr)
        {
            _headerLocalize.Term = header;
            _descrLocalize.Term = descr;
            _panel.SetActive(true);
        }
        
        public void OpenWindowSetText(string header, string descr)
        {
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