using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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
        [Required]
        public BattlePlayerStatsContainerUIMono PlayerStatsContainer_Attack;

        [Required]
        public BattlePlayerStatsContainerUIMono PlayerStatsContainer_Defence;

        [SerializeField]
        [Required]
        private List<BattleTacticsSlotUIMono> _battleTacticsSlotList = new ();

        public void OnEnable()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
        }

        public void EnableTacticsUI()
        {
            _panel.SetActive(true);
            _background.SetActive(true);
        }

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