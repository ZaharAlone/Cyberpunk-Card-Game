using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Core.Battle.TacticsMode
{
    public class BattlePlayerStatsContainerUIMono : MonoBehaviour
    {
        [Header("Player Avatar")]
        [SerializeField]
        [Required]
        private Image _playerAvatar;

        [Header("Stats")]
        [SerializeField]
        [Required]
        private TextMeshProUGUI _power_stat_text;
        
        [SerializeField]
        [Required]
        private TextMeshProUGUI _kill_stat_text;

        [SerializeField]
        [Required]
        private TextMeshProUGUI _defence_stat_text;

        public void SetAvatarPlayer(Sprite avatar)
        {
            _playerAvatar.sprite = avatar;
        }

        public void SetStats(string power, string kill, string defence)
        {
            _power_stat_text.text = power;
            _kill_stat_text.text = kill;
            _defence_stat_text.text = defence;
        }
    }
}