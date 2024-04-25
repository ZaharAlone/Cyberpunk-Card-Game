using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.UI
{
    public class PlayerEnemyTurnActionUIMono : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private GameObject _panelGO;
        [Required]
        [SerializeField]
        private RectTransform _panelTransform;
        [Required]
        [SerializeField]
        private Image _avatarPlayer;
        [Required]
        [SerializeField]
        private TextMeshProUGUI _actionPlayerText;
        [Required]
        [SerializeField]
        private TextMeshProUGUI _namePlayerText;
        [Required]
        [SerializeField]
        private Image _iconsPlayerUnit;
        [Required]
        [SerializeField]
        private Transform _containerCard;

        [SerializeField]
        private Vector2 _startPosition = new Vector2(127.5f, 110);

        private List<CardMono> _cardsHand = new List<CardMono>();
        private Sequence _sequence;
        
        public void SetViewPlayer(Sprite avatar, string playerName, Sprite iconsUnit, Color32 colorUnit)
        {
            _avatarPlayer.sprite = avatar;
            _namePlayerText.text = playerName;
            _iconsPlayerUnit.sprite = iconsUnit;
            _iconsPlayerUnit.color = colorUnit;
        }

        public void SetHeaderView(string actionText)
        {
            _actionPlayerText.text = actionText;
        }

        public void EnableFrame()
        {
            _panelGO.SetActive(true);
            _sequence = DOTween.Sequence();
            _sequence.Append(_panelTransform.DOAnchorPos(new Vector2(0, _panelTransform.anchoredPosition.y), 0.3f));
        }

        public void DisableFrame()
        {
            _sequence = DOTween.Sequence();
            _sequence.Append(_panelTransform.DOAnchorPos(new Vector2(-300, _panelTransform.anchoredPosition.y), 0.4f))
                .OnComplete(()=> OnDisableFrame());
        }

        private void OnDisableFrame()
        {
            _panelGO.SetActive(false);
        }

        public CardMono CreateNewCard(CardMono cardView)
        {
            var cardMono = Instantiate(cardView, _containerCard);
            cardMono.RectTransform.anchoredPosition = _startPosition;
            _cardsHand.Add(cardMono);
            AnimationsCardShift();
            return cardMono;
        }
        
        private void AnimationsCardShift()
        {
            if (_cardsHand.Count <= 1)
                return;
            
            for (int i = 0; i < _cardsHand.Count - 1; i++)
            {
                var sequence = DOTween.Sequence();

                var newPos = new Vector2(_cardsHand[i].RectTransform.anchoredPosition.x - 25, _cardsHand[i].RectTransform.anchoredPosition.y);
                sequence.Append(_cardsHand[i].RectTransform.DOAnchorPos(newPos, 0.35f)).OnComplete(() => sequence.Kill());
            }    
        }

        public void ClearContainerCard()
        {
            foreach (var card in _cardsHand)
            {
                Destroy(card.gameObject);
            }
            
            _cardsHand.Clear();
        }
        
        public void OnDestroy()
        {
            ClearContainerCard();
        }
    }
}