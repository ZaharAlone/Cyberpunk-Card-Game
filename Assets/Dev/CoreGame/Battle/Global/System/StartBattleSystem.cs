using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Global;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class StartBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.OnClickStartBattle += StartBattle;
        }

        private void StartBattle()
        {
            var currentBattleData = _dataWorld.OneData<BattleCurrentData>();
            
            //SelectActionPlayers(currentBattleData.AttackingPlayer, true);
            //SelectActionPlayers(currentBattleData.DefendingPlayer, false);
        }

        private void SelectActionPlayers(PlayerInBattleComponent playerInBattle, bool isAttacking)
        {
            /*
            var playerIsAI = BattleSupport.ControlEntityIsAI(playerInBattle.PlayerControlEntity);
            var playerIsNeutral = playerInBattle.PlayerControlEntity == PlayerOrAI.None;
            
            if (playerIsAI)
            {
                //var selectAITactics = BattleAction.SelectTacticsAI.Invoke(isAttacking);
                //ApplyAISelectTactics(playerInBattle, selectAITactics);
            }
            else if (playerIsNeutral)
            {
                
            }*/
        }
/*
        private void ApplyAISelectTactics(PlayerInBattleComponent playerInBattle, SelectTacticsAndCardAIDTO selectTactics)
        {
            var targetCardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == selectTactics.GUIDCard)
                .SelectFirstEntity();

            var targetCardComponent = targetCardEntity.GetComponent<CardComponent>();
            
            
        }

        private void PlayerApplySelectTactics()
        {
            
        }
*/
        public void Destroy()
        {
            BattleTacticsUIAction.OnClickStartBattle -= StartBattle;
        }
    }
}