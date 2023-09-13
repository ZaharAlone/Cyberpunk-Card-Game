using System;
using System.Threading.Tasks;
using UnityEngine;
using I2.Loc;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace CyberNet.Core.UI
{
    public class ChangeRoundUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject NewRoundGO;
        [SerializeField]
        private RectTransform NewRoundRect;
        [SerializeField]
        private TextMeshProUGUI TextRound;
        [SerializeField]
        private Image AvatarImage;
        
        [SerializeField]
        private LocalizedString PassToLoc;

        private Sequence _sequence;
        private float _deltaSize;
        
        public void Awake()
        {
            _deltaSize = Screen.currentResolution.width / 2 + NewRoundRect.sizeDelta.x / 2;
        }

        public void NewRoundView(Sprite avatar, string playerName)
        {
            TextRound.text = PassToLoc + " " + playerName;
            AvatarImage.sprite = avatar;
            
            NewRoundGO.SetActive(true);
            NewRoundRect.position = new Vector3(-_deltaSize, NewRoundRect.position.y, NewRoundRect.position.z);
            _sequence = DOTween.Sequence();
            _sequence.Append(NewRoundRect.DOAnchorPos(new Vector2(0,0), 0.7f))
                .AppendInterval(2f)
                .Append(NewRoundRect.DOAnchorPos(new Vector2(_deltaSize, 0), 0.7f))
                .OnComplete(()=> CompleteAnimatios());
        }

        private void CompleteAnimatios()
        {
            NewRoundGO.SetActive(false);
        }
    }
}