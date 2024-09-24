using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.Battle.TacticsMode
{
    public class BattleTacticsModeUIMono : MonoBehaviour
    {
        [Header("Global")]
        [SerializeField]
        [Required]
        private GameObject _background;

        [SerializeField]
        [Required]
        private GameObject _panel;

        [Header("Elements")]
        [SerializeField]
        [Required]
        private BattlePlayerStatsContainerUIMono _playerStatsContainer_Left;

        [SerializeField]
        [Required]
        private BattlePlayerStatsContainerUIMono _playerStatsContainer_Right;

        [SerializeField]
        [Required]
        private List<BattleTacticsSlotUIMono> battleTacticsSlotList = new ();
        
        public void OnClickNextLeftBattleTactics()
        {
            BattleTacticsUIAction.NextLeftBattleTactics?.Invoke();
        }
        
        public void OnClickNextRightBattleTactics()
        {
            BattleTacticsUIAction.NextRightBattleTactics?.Invoke();
        }
    }
}