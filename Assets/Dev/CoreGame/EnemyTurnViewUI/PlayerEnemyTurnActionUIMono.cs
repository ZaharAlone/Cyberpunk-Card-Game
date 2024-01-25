using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace CyberNet.Core.UI
{
    public class PlayerEnemyTurnActionUIMono : MonoBehaviour
    {
        [FormerlySerializedAs("_panel")]
        [SerializeField]
        private GameObject _panelGO;
        [SerializeField]
        private RectTransform _panelTransform;
        [SerializeField]
        private Image _avatarPlayer;
        [SerializeField]
        private TextMeshProUGUI _actionPlayerText;
        [SerializeField]
        private TextMeshProUGUI _namePlayerText;
        [SerializeField]
        private Image _iconsPlayerUnit;
        [SerializeField]
        private Transform _containerCard;

        private Sequence _sequence;
        
        public void SetViewPlayer(Sprite avatar, string playerName, string actionText, Sprite iconsUnit, Color32 colorUnit)
        {
            _avatarPlayer.sprite = avatar;
            _namePlayerText.text = playerName;
            _actionPlayerText.text = actionText;
            _iconsPlayerUnit.sprite = iconsUnit;
            _iconsPlayerUnit.color = colorUnit;
        }

        public void EnableFrame()
        {
            _panelGO.SetActive(true);
            _sequence = DOTween.Sequence();
            _sequence.Append(_panelTransform.DOAnchorPos(new Vector2(0, _panelTransform.anchoredPosition.y), 0.2f));
        }

        public void DisableFrame()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(_panelTransform.DOAnchorPos(new Vector2(-300, _panelTransform.anchoredPosition.y), 0.2f))
                .OnComplete(()=> OnDisableFrame());
        }

        private void OnDisableFrame()
        {
            _panelGO.SetActive(false);
        }

        public Transform GetCardContainerTransform()
        {
            return _containerCard;
        }

        public void ClearContainerCard()
        {
            foreach (Transform child in _containerCard)
            {
                Destroy(child.gameObject);
            }
        }
    }
}