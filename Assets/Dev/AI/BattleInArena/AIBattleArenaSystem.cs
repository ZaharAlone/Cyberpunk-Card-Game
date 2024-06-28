using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Arena;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.City;
using CyberNet.Core.EnemyTurnView;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Global;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CyberNet.Core.AI.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class AIBattleArenaSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AIBattleArenaAction.CheckEnemyBattle += CheckEnemy;
            AIBattleArenaAction.StartAIRound += BattleAILogic;
        }
        
        //Перед началом боя проверяем есть ли у нас противники игроки
        //Эта логика вызывается когда на территорию нападает бот
        private void CheckEnemy()
        {
            var unitEntities = _dataWorld.Select<UnitInBattleArenaComponent>().GetEntities();

            //Есть ли противники игроки
            var isEnemyPlayers = false;

            foreach (var unitEntity in unitEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                if (unitComponent.PlayerControl != PlayerControlEntity.PlayerControl)
                    continue;
                
                var playerEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == unitComponent.PowerSolidPlayerID)
                    .SelectFirstEntity();
                if (playerEntity.GetComponent<PlayerComponent>().playerOrAI == PlayerOrAI.Player)
                {
                    isEnemyPlayers = true;
                    break;
                }
            }

            if (!isEnemyPlayers)
            {
                ref var arenaData = ref _dataWorld.OneData<ArenaData>();
                arenaData.IsShowVisualBattle = false;
                ArenaAction.CreateBattleData?.Invoke();
                ArenaAction.FindPlayerInCurrentRound();
                ArenaAction.CreateUnitInArena?.Invoke();
                BattleAILogic();
            }
            else
            {
                MapMoveUnitsAction.StartArenaBattle?.Invoke();
                EnemyTurnViewUIAction.ForceHideView?.Invoke();
            }
        }

        //Логика бота
        private void BattleAILogic()
        {
            CheckUseCardInHand();
            SelectTargetForAttack();

            AttackSelectEnemy();
        }

        //Бот смотрит карты в руке, может ли что то разыграть, если да, то смотрит насколько это эффективно
        //Если эффективно - разыгрывает
        private bool CheckUseCardInHand()
        {
            //Возможно вначале нужно посмотреть есть ли карты в руке у игрока
            
            var countCardCanUse = _dataWorld.Select<CardCanUseComponent>()
                .With<CardHandComponent>()
                .Count();

            if (countCardCanUse == 0)
                return false;

            var cardEntities = _dataWorld.Select<CardComponent>()
                .With<CardCanUseComponent>()
                .With<CardHandComponent>()
                .GetEntities();

            // TODO Дописать когда у нас появятся карты для арены
            //Select potential card, playing
            
            return true;
        }

        //Бот выбирает цель для атаки
        private void SelectTargetForAttack()
        {
            //Находим первого попавшегося противника на арене
            var selectEnemyPlayerEntity = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Without<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var selectEnemyPlayerComponent = selectEnemyPlayerEntity.GetComponent<PlayerArenaInBattleComponent>();

            var selectEnemyUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == selectEnemyPlayerComponent.PlayerID)
                .SelectFirstEntity();
            
            selectEnemyUnitEntity.AddComponent(new ArenaSelectUnitForAttackComponent());
        }
        
        //Атакуем выбранного противника
        //Логика разная в зависимости от того:
        //есть ли на арене игрок или одни боты (показываем вьюху или нет)
        private void AttackSelectEnemy()
        {
            var selectEnemyUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            
            var showViewBattle = _dataWorld.OneData<ArenaData>().IsShowVisualBattle;
            
            if (showViewBattle)
            {
                var colorsConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ColorsGameConfigSO;
                var selectEnemyUnitComponent = selectEnemyUnitEntity.GetComponent<ArenaUnitComponent>();
            
                selectEnemyUnitComponent.UnitArenaMono.UnitPointVFXMono.SetColor(colorsConfig.SelectWrongTargetRedColor, true);
                selectEnemyUnitComponent.UnitArenaMono.UnitPointVFXMono.EnableEffect();
            
                //UnitArenaAction.EndShootingAnimations += 
                
                ArenaAction.ArenaUnitStartShooting?.Invoke();
            }
            else
            {
                var isDiscardCard = false;
                if (ArenaAction.CheckBlockAttack.Invoke())
                { 
                    isDiscardCard = ArenaAction.CheckReactionsShooting.Invoke();
                }
                
                if (!isDiscardCard)
                {
                    KillUnitWithoutVisual();
                }
                
                EndRound();
            }
        }
        
        /// <summary>
        /// Kills a unit without displaying any visual effects.
        /// </summary>
        private void KillUnitWithoutVisual()
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitMapComponent = targetUnitEntity.GetComponent<UnitMapComponent>();
            
            Object.Destroy(targetUnitMapComponent.UnitIconsGO);
            
            targetUnitEntity.Destroy();
        }

        /// <summary>
        /// Ends the current round of the battle.
        /// </summary>
        private void EndRound()
        {
            var isVisual = _dataWorld.OneData<ArenaData>().IsShowVisualBattle;

            if (isVisual)
            {
                ArenaUIAction.StartNewRoundUpdateOrderPlayer?.Invoke();
                ArenaAction.FinishRound?.Invoke();
            }
            else
            {
                //Проверяем не закончилась ли битва
                //Если закончилась заканчиваем, если нет - обновляем порядок хода игроков.
                var isEnd = ArenaAction.CheckFinishArenaBattle.Invoke();

                if (isEnd)
                {
                    Debug.Log("Battle is finish");
                    FinishBattleNotVisual();
                }
                else
                {
                    Debug.Log("Init new round battle");
                    ArenaAction.DeselectPlayer?.Invoke();
                    ArenaAction.UpdateTurnOrderArena?.Invoke();
                    ArenaAction.FindPlayerInCurrentRound();
                    Debug.Log("Start new round battle");
                    BattleAILogic();
                }
            }
        }

        private void FinishBattleNotVisual()
        {
            var playerInArenaEntities = _dataWorld.Select<PlayerArenaInBattleComponent>().GetEntities();

            foreach (var playerEntity in playerInArenaEntities)
            {
                playerEntity.Destroy();
            }

            var unitInArenaEntities = _dataWorld.Select<ArenaUnitComponent>().GetEntities();

            foreach (var unitEntity in unitInArenaEntities)
            {
                unitEntity.RemoveComponent<ArenaUnitComponent>();
                unitEntity.RemoveComponent<ArenaUnitCurrentComponent>();
                unitEntity.RemoveComponent<UnitInBattleArenaComponent>();
            }

            var unitSelectForAttack = _dataWorld.Select<ArenaSelectUnitForAttackComponent>().GetEntities();
            foreach (var unitEntity in unitSelectForAttack)
            {
                unitEntity.RemoveComponent<ArenaSelectUnitForAttackComponent>();
            }

            _dataWorld.RemoveOneData<ArenaRoundData>();
            
            BotAIAction.ContinuePlayingCards?.Invoke();
        }
        
        public void Destroy()
        {
            AIBattleArenaAction.CheckEnemyBattle -= CheckEnemy;
            AIBattleArenaAction.StartAIRound -= BattleAILogic;
        }
    }
}