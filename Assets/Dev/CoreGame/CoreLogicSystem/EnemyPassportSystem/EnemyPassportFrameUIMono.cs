using System.Collections.Generic;
using CyberNet.Core.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Core.EnemyPassport
{
    public class EnemyPassportFrameUIMono : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private EnemyPassportInteractiveMono _enemyPassportInteractiveMono;
        
        [Header("View")]
        [SerializeField]
        private Image _avatar;
        [SerializeField]
        private TextMeshProUGUI _namePlayer;
        [SerializeField]
        private Image _iconsUnit;
        [SerializeField]
        private TextMeshProUGUI _countCardInHandText;
        [SerializeField]
        private TextMeshProUGUI _countCardInDiscardText;
        [SerializeField]
        private TextMeshProUGUI _countLeftUnitText;
        [SerializeField]
        private List<GameObject> _progressWinGame;
        [SerializeField]
        private Image _iconsProgressGame;
        [SerializeField]
        private GameObject _vfxTurnPlayer;
        
        [SerializeField]
        private GameObject _effectSelectPlayer;
        [SerializeField]
        [Required]
        private TextMeshProUGUI _victoryPointText;
        [SerializeField]
        [Required]
        private GameObject _discardCardIcons;
        [SerializeField]
        [Required]
        private TextMeshProUGUI _countDiscardCardText;
        
        private int _playerID;
        
        public void SetViewPlayer(Sprite imageAvatar, string namePlayer, Sprite iconsUnit, Color32 colorUnit)
        {
            _avatar.sprite = imageAvatar;
            _namePlayer.text = namePlayer;
            _iconsUnit.sprite = iconsUnit;
            _iconsUnit.color = colorUnit;
        }

        public void SetPlayerID(int playerID)
        {
            _playerID = playerID;
            _enemyPassportInteractiveMono.SetPlayerID(playerID);
        }

        public int GetPlayerID()
        {
            return _playerID;
        }

        public void SetStats(int countCardHand, int countCardDiscard,int countUnit)
        {
            _countLeftUnitText.text = countUnit.ToString();
            _countCardInHandText.text = countCardHand.ToString();
            _countCardInDiscardText.text = countCardDiscard.ToString();
        }

        public void SetCountVictoryPoint(int countVictoryPoint, int countTargetVictoryPoint)
        {
            var textVP = countVictoryPoint + "/" + countTargetVictoryPoint;
            _victoryPointText.text = textVP;
        }

        public void DiscardCardStatusLeftPlayer(int count)
        {
            if (count == 0)
            {
                _discardCardIcons.SetActive(false);
                _countDiscardCardText.text = "";
            }
            else
            {
                _discardCardIcons.SetActive(true);
                var countDiscardCardText = count > 1 ? count.ToString() : "";
                _countDiscardCardText.text = countDiscardCardText;
            }
        }
        
        public void OnEffectSelectPlayerStatus()
        {
            _effectSelectPlayer.SetActive(true);
        }

        public void OffEffectSelectPlayerStatus()
        {
            _effectSelectPlayer.SetActive(false);
        }

        public void EnableCurrentTurnPlayer(bool status)
        {
            _vfxTurnPlayer.SetActive(status);
        }
    }
}