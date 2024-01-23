using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace CyberNet.Core.UI
{
    public class PlayerEnemyTurnActionUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _panel;
        [SerializeField]
        private Image _avatarPlayer;
        [SerializeField]
        private TextMeshProUGUI _actionPlayerText;
        [SerializeField]
        private TextMeshProUGUI _namePlayerText;
        [SerializeField]
        private Image _iconsPlayerUnit;

        public void SetViewPlayer(Sprite avatar, string playerName, string actionText, Sprite iconsUnit, Color32 colorUnit)
        {
            _avatarPlayer.sprite = avatar;
            _namePlayerText.text = playerName;
            _actionPlayerText.text = actionText;
            _iconsPlayerUnit.sprite = iconsUnit;
            _iconsPlayerUnit.color = colorUnit;
        }

        public void EnableFrame(bool status)
        {
            _panel.SetActive(status);
        }
    }
}