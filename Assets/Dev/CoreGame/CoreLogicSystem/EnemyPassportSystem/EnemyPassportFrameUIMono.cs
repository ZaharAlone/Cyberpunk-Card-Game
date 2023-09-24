using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.EnemyPassport
{
    public class EnemyPassportFrameUIMono : MonoBehaviour
    {
        [SerializeField] private Image Avatar;
        [SerializeField] private Image StatsFrame;
        [SerializeField] private TextMeshProUGUI StatsText;
        [SerializeField] private GameObject EffectSelectPlayer;
        [SerializeField] private GameObject DiscardCardImageStatus;

        private int _playerID;
        
        public void SetAvatar(Sprite imageAvatar)
        {
            Avatar.sprite = imageAvatar;
        }

        public void SetPlayerID(int playerID)
        {
            _playerID = playerID;
        }

        public void SetStatsColor(Color32 color)
        {
            StatsFrame.color = color;
            StatsText.color = color;
        }

        public void SetStats(int value)
        {
            StatsText.text = value.ToString();
        }

        public void OnSelectPlayer()
        {
            EnemyPassportAction.SelectPlayer?.Invoke(_playerID);
        }

        public void OnDiscardCardStatus()
        {
            DiscardCardImageStatus.SetActive(true);
        }

        public void OffDiscardCardStatus()
        {
            DiscardCardImageStatus.SetActive(false);
        }
        
        public void OnEffectSelectPlayerStatus()
        {
            EffectSelectPlayer.SetActive(true);
        }

        public void OffEffectSelectPlayerStatus()
        {
            EffectSelectPlayer.SetActive(false);
        }
    }
}