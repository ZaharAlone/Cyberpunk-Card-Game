using System.Collections.Generic;
using CyberNet.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace CyberNet.Core.EnemyPassport
{
    public class EnemyPassportFrameUIMono : MonoBehaviour
    {
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
        private List<GameObject> DiscardCardImages = new List<GameObject>();
        [FormerlySerializedAs("EnemyPassportControlTerritoryView")]
        [SerializeField]
        private PlayerPassportValueWinProgressUIMono _enemyPassportControlTerritoryView;

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

        public void SetViewCountControlTerritory(int countBase)
        {
            Debug.LogError($"Set view player count {countBase}");
            _enemyPassportControlTerritoryView.SetCountValue(countBase);
        }

        public void OnSelectPlayer()
        {
            EnemyPassportAction.SelectPlayer?.Invoke(_playerID);
        }

        public void DiscardCardStatus(int count)
        {
            for (int i = 0; i < DiscardCardImages.Count; i++)
            {
                DiscardCardImages[i].SetActive(count > i);
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