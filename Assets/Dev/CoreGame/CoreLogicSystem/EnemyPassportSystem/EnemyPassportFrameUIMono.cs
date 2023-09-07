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

        public void SetAvatar(Sprite imageAvatar)
        {
            Avatar.sprite = imageAvatar;
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
    }
}