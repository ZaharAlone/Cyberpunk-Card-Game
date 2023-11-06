using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Core.UI.CorePopup
{
    /// <summary>
    ///     For different kind of hints
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class CoreElementInfoPopupUIMono : MonoBehaviour
    {
        [Header("Links Element")]
        [SerializeField]
        private GameObject _popup;
        [SerializeField]
        private RectTransform _popupRect;

        [SerializeField]
        private Localize _headerText;
        [SerializeField]
        private Localize _descrText;
        [SerializeField]
        private TextMeshProUGUI _artisticText;

        private readonly Vector3[] _corners = new Vector3[4];

        private void Awake()
        {
            _popup.SetActive(false);
        }

        public void SetView(string descrText, string headerText = null, string artisticText = null)
        {
            _descrText.Term = descrText;

            if (headerText != null)
            {
                _headerText.Term  = headerText;
                _headerText.gameObject.SetActive(true);
            }
            else
            {
                _headerText.gameObject.SetActive(false);
            }
            
            if (artisticText != null)
            {
                var text = LocalizationManager.GetTranslation(artisticText);
                _artisticText.text = "\"" + text + "\"";
                _artisticText.gameObject.SetActive(true);
            }
            else
            {
                _artisticText.gameObject.SetActive(false);
            }
            
            _popup.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_popupRect);
        }

        public void PositioningLeftRight(RectTransform target, float offset, float cardScale = 1f)
        {
            var targetPosition = target.position;
            targetPosition.x += target.sizeDelta.x * cardScale / 2 + offset;
            targetPosition.y += target.sizeDelta.y * cardScale / 2;
            
            _popupRect.position = targetPosition;
            _popupRect.GetWorldCorners(_corners);
            
            if (_corners[2].x > Screen.width)
            {
                var newPosX = target.sizeDelta.x * cardScale + offset * 2 + _popupRect.sizeDelta.x;
                _popupRect.position -= new Vector3(newPosX, 0, 0);
            }
        }
        
        public void PositioningUp(RectTransform target, float offset)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_popupRect);
            var targetPosition = target.position;
            targetPosition.x -= target.sizeDelta.x / 2 + _popupRect.sizeDelta.x / 2;
            targetPosition.y += target.sizeDelta.y + _popupRect.sizeDelta.y + offset;
            
            _popupRect.position = targetPosition;
            _popupRect.GetWorldCorners(_corners);
            
            if (_corners[2].x > Screen.width)
            {
                var width = _corners[2].x - _corners[0].x;
                _popupRect.position -= new Vector3(_corners[0].x - Screen.width + width + offset, 0, 0);
            }
        }

        public void ClosePopup()
        {
            _popup.SetActive(false);
        }
    }
}