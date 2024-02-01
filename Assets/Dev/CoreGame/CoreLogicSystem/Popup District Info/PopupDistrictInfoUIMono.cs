using System;
using System.Threading.Tasks;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Core.UI.PopupDistrictInfo
{
    public class PopupDistrictInfoUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private RectTransform _rectPanel;
        [SerializeField]
        private RectTransform _rectHeader;
        [SerializeField]
        private HorizontalLayoutGroup _headerLayout;
        [SerializeField]
        private VerticalLayoutGroup _descrLayout;
        
        [SerializeField]
        private TextMeshProUGUI _headerText;
        [SerializeField]
        private TextMeshProUGUI _descrText;

        [SerializeField]
        private Image _iconsFraction;
        [SerializeField]
        private TextMeshProUGUI _namePlayerControlDistrictText;

        private bool _isOpenPopup;
        
        public void OnEnable()
        {
            _isOpenPopup = false;
            _panel.SetActive(false);
        }

        public void OpenPopup(string header, string descr)
        {
            _headerText.text = LocalizationManager.GetTranslation(header);
            var locDescr = LocalizationManager.GetTranslation(descr);
            _descrText.text = "\"" + locDescr + "\"";

            if (!_isOpenPopup)
            {
                _isOpenPopup = true;
                _panel.SetActive(true);
            }
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectPanel);
        }

        public void SetFractionView(Sprite icons, Color32 colorPlayer, string namePlayer)
        {
            _iconsFraction.gameObject.SetActive(icons != null);
            
            _iconsFraction.sprite = icons;
            _iconsFraction.color = colorPlayer;
            _namePlayerControlDistrictText.text = namePlayer;
            _namePlayerControlDistrictText.color = colorPlayer;
        }

        public void ClosePopup()
        {
            _isOpenPopup = false;
            _panel.SetActive(false);
        }
    }
}