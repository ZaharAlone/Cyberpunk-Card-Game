using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.UI.CardPopup
{
    /// <summary>
    ///     For different kind of hints
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CardPopupMono : MonoBehaviour
    {
        [Header("Links Element")]
        [SerializeField]
        private GameObject _popup;
        [SerializeField]
        private RectTransform _popupRect;

        [SerializeField]
        private Localize _descrTextCard;
        [SerializeField]
        private TextMeshProUGUI _artisticTextCard;

        [Header("Settings")]
        [SerializeField]
        private int _offsetX = 10;
        
        private readonly Vector3[] _corners = new Vector3[4];

        private void Awake()
        {
            _popup.SetActive(false);
        }

        public void SetView(string descrText, string artisticText = "")
        {
            _descrTextCard.Term = descrText;

            if (artisticText != "")
            {
                var text = LocalizationManager.GetTranslation(artisticText);
                _artisticTextCard.text = "\"" + text + "\"";
                _artisticTextCard.gameObject.SetActive(true);
            }
            else
            {
                _artisticTextCard.gameObject.SetActive(false);
            }
            
            _popup.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_popupRect);
        }

        public void Positioning(RectTransform target, float cardScale = 1f)
        {
            var targetPosition = target.position;
            targetPosition.x += target.sizeDelta.x * cardScale / 2 + _offsetX;
            targetPosition.y += target.sizeDelta.y * cardScale / 2;
            
            _popupRect.position = targetPosition;
            _popupRect.GetWorldCorners(_corners);
            
            if (_corners[2].x > Screen.width)
            {
                var newPosX = target.sizeDelta.x * cardScale + _offsetX * 2 + _popupRect.sizeDelta.x;
                _popupRect.position -= new Vector3(newPosX, 0, 0);
            }
        }

        public void ClosePopup()
        {
            _popup.SetActive(false);
        }
    }
}