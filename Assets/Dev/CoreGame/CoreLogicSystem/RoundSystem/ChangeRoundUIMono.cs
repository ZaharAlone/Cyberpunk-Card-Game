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
            TextRound.text = LocalizationManager.GetTranslation(PassToLoc) + " " + playerName;
            AvatarImage.sprite = avatar;
            
            NewRoundGO.SetActive(true);
            NewRoundRect.position = new Vector3(-_deltaSize, NewRoundRect.position.y, NewRoundRect.position.z);
            _sequence = DOTween.Sequence();
            _sequence.Append(NewRoundGO.transform.DOMoveX(0f, 0.7f))
                .AppendInterval(1.2f)
                .Append(NewRoundGO.transform.DOMoveX(_deltaSize, 0.7f))
                .OnComplete(()=> CompleteAnimatios());
        }

        private void CompleteAnimatios()
        {
            NewRoundGO.SetActive(false);
        }
    }
}