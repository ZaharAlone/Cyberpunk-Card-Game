using System;
using System.Threading.Tasks;
using UnityEngine;
using I2.Loc;
using DG.Tweening;
using TMPro;

namespace CyberNet.Core.UI
{
    public class ChangeRoundUIMono : MonoBehaviour
    {
        public GameObject NewRoundGO;
        public RectTransform NewRoundRect;
        public TextMeshProUGUI TextRound;
        
        public LocalizedString PlayerRoundLoc;
        public LocalizedString EnemyRoundLoc;

        private Sequence _sequence;
        private float _deltaSize;
        
        public void Start()
        {
            _deltaSize = Screen.currentResolution.width / 2 + NewRoundRect.sizeDelta.x / 2;
        }

        public void PlayerRound()
        {
            TextRound.text = PlayerRoundLoc;
            NewRoundAnimations();
        }

        public void EnemyRound()
        {
            TextRound.text = EnemyRoundLoc;
            NewRoundAnimations();
        }

        private void NewRoundAnimations()
        {
            NewRoundGO.SetActive(true);
            NewRoundRect.position = new Vector3(-_deltaSize, NewRoundRect.position.y, NewRoundRect.position.z);
            _sequence = DOTween.Sequence();
            _sequence.Append(NewRoundGO.transform.DOMoveX(0f, 1f))
                .AppendInterval(1.5f)
                .Append(NewRoundGO.transform.DOMoveX(_deltaSize, 1f))
                .OnComplete(()=> CompleteAnimatios());
        }

        private void CompleteAnimatios()
        {
            NewRoundGO.SetActive(false);
        }
    }
}