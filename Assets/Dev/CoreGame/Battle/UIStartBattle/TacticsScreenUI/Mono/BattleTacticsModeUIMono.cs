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

        [Required]
        public RectTransform PointToTargetCard;
        
        [Required]
        public List<BattleTacticsSlotUIMono> BattleTacticsSlotList = new ();

        [Required]
        public Transform CardsContainer;
        
        public void OnEnable()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
        }

        public void ShowTacticsUI()
        {
            _panel.SetActive(true);
            _background.SetActive(true);
        }
        
        public void HideTacticsUI()
        {
            _panel.SetActive(false);
            _background.SetActive(false);
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