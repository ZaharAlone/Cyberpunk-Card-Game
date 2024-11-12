using System;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.Battle
{
    public class WinLoseBattleUIMono : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private GameObject _popup;

        [SerializeField]
        [Required]
        private GameObject _youWinGO;

        [SerializeField]
        [Required]
        private GameObject _youLoseGO;

        public void OnEnable()
        {
            _popup.SetActive(false);
        }

        public void ShowPopupWin()
        {
            _popup.SetActive(true);
            _youWinGO.SetActive(true);
            _youLoseGO.SetActive(false);
            DisablePopup();
        }
        
        public void ShowPopupLose()
        {
            _popup.SetActive(true);
            _youWinGO.SetActive(false);
            _youLoseGO.SetActive(true);
            DisablePopup();
        }

        private async void DisablePopup()
        {
            await Task.Delay(5000);
            _popup.SetActive(false);
        }
    }
}