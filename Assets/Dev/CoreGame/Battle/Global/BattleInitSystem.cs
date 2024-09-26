using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AI.Battle;
using CyberNet.Core.Arena;
using CyberNet.Core.Battle;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Map;
using CyberNet.Global;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class BattleInitSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleAction.EndMovePlayerToNewZone += StartBattle;
            BattleAction.FinishBattle += FinishBattle;
        }

        private void StartBattle()
        {
            CreateBattleData();
            
            var currentPlayerType = _dataWorld.OneData<RoundData>().playerOrAI;

            if (currentPlayerType != PlayerOrAI.Player)
            {
                //TODO начинаем битву между ботами
                AIBattleAction.CheckEnemyBattle?.Invoke();
            }
            else
            {
                StartArenaBattle();
            }
        }
        
        private void CreateBattleData()
        {
            var battleData = new BattleCurrentData();
            
            var atackingPlayerStruct = new PlayerInBattleStruct();
            var defencePlayerStruct = new PlayerInBattleStruct();
            
            var countAttackingUnit = _dataWorld.Select<UnitInBattleArenaComponent>()
                .Where<UnitInBattleArenaComponent>(unit => unit.Attacking)
                .Count();
            
            var countDefenceUnit = _dataWorld.Select<UnitInBattleArenaComponent>()
                .Where<UnitInBattleArenaComponent>(unit => !unit.Attacking)
                .Count();
        }

        private void StartArenaBattle()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.CurrentGameStateMapVSArena = GameStateMapVSArena.Arena;
            ref var arenaData = ref _dataWorld.OneData<ArenaData>();
            arenaData.IsShowVisualBattle = true;
            
            BattleAction.OpenTacticsScreen?.Invoke();   
        }

        private void FinishBattle()
        {
            
        }

        public void Destroy()
        {
            BattleAction.EndMovePlayerToNewZone -= StartBattle;
            BattleAction.FinishBattle -= FinishBattle;
        }
    }
}