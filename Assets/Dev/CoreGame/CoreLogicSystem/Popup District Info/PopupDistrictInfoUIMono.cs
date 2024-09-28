using I2.Loc;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Core.UI.PopupDistrictInfo
{
    public class PopupDistrictInfoUIMono : MonoBehaviour
    {
        [Header("Global")]
        [SerializeField]
        [Required]
        private GameObject _panel;
        
        [Header("View district info")]
        [SerializeField]
        [Required]
        private TextMeshProUGUI _headerText;
        [SerializeField]
        [Required]
        private TextMeshProUGUI _descrText;

        [Header("View player control")]
        [SerializeField]
        [Required]
        private Image _iconsFraction;
        [SerializeField]
        [Required]
        private TextMeshProUGUI _namePlayerControlDistrictText;

        [Header("View bonus district")]
        [SerializeField]
        [Required]
        private Image _iconsBonus;
        [SerializeField]
        [Required]
        private TextMeshProUGUI _countBonusText;
        
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
        }

        public void SetFractionView(Sprite icons, Color32 colorPlayer, string namePlayer)
        {
            _iconsFraction.gameObject.SetActive(icons != null);
            
            _iconsFraction.sprite = icons;
            _iconsFraction.color = colorPlayer;
            _namePlayerControlDistrictText.text = namePlayer;
            _namePlayerControlDistrictText.color = colorPlayer;
        }

        public void SetBonus(Sprite icons, Color32 colorBonus, int countBonus)
        {
            _iconsBonus.sprite = icons;

            if (countBonus > 1)
            {
                _countBonusText.gameObject.SetActive(true);
                _countBonusText.text = countBonus.ToString();
                _countBonusText.color = colorBonus;
            }
            else
                _countBonusText.gameObject.SetActive(false);
        }
        
        public void ClosePopup()
        {
            _isOpenPopup = false;
            _panel.SetActive(false);
        }
    }
}