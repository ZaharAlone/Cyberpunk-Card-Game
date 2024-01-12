using CyberNet.Core.AI.Arena;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaSetupRoundVisualSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.UpdateRound += UpdateRoundVisual;
            ArenaAction.FinishRound += FinishRound;
        }

        public void Init()
        {
            _dataWorld.CreateOneData(new ArenaRoundData());
            UpdateRoundVisual();
        }

        public void UpdateRoundVisual()
        {
            ref var roundData = ref _dataWorld.OneData<ArenaRoundData>();
            roundData.ArenaCurrentStage = ArenaCurrentStageEnum.Action;

            //Отключаем визуал юнитов игрока ходившего раньше, и он перестает быть текущим игроком
            DeselectPlayer();
            
            // Ищем следующего игрока, и включаем весь его визуал
            ArenaAction.FindPlayerInCurrentRound();
            SwitchRoundCamera();
            SelectCurrentUnitVisual();

            // Выдаем контроль игроку, или AI
            EnableControlPlayer();
        }
        private void DeselectPlayer()
        {
            var unitsEntities = _dataWorld.Select<ArenaUnitComponent>()
                .GetEntities();

            foreach (var unitEntity in unitsEntities)
            {
                var unitComponent = unitEntity.GetComponent<ArenaUnitComponent>();
                unitComponent.UnitArenaMono.UnitPointVFXMono.DisableEffect();

                if (unitEntity.HasComponent<ArenaUnitCurrentComponent>())
                    unitEntity.RemoveComponent<ArenaUnitCurrentComponent>();
            }

            var isEntityCurrentPlayer = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .With<CurrentPlayerComponent>()
                .TrySelectFirstEntity(out var currentPlayerEntity);

            if (isEntityCurrentPlayer)
                currentPlayerEntity.RemoveComponent<CurrentPlayerComponent>();
        }

        private void SwitchRoundCamera()
        {
            var roundData = _dataWorld.OneData<ArenaRoundData>();
            var playersInBattleEntity = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Where<PlayerArenaInBattleComponent>(player => player.PlayerID == roundData.CurrentPlayerID 
                    && player.PlayerControlEntity == roundData.PlayerControlEntity)
                .SelectFirstEntity();

            var playerComponent = playersInBattleEntity.GetComponent<PlayerArenaInBattleComponent>();
            var arenaMono = _dataWorld.OneData<ArenaData>().ArenaMono;

            if (playerComponent.Forwards)
            {
                arenaMono.ArenaCameraMono.SetLeftCamera();
            }
            else
            {
                arenaMono.ArenaCameraMono.SetRightCamera();
            }
        }

        private void SelectCurrentUnitVisual()
        {
            var roundData = _dataWorld.OneData<ArenaRoundData>();
            var unitsEntities = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == roundData.CurrentPlayerID
                    && unit.playerControlEntity == roundData.PlayerControlEntity)
                .GetEntities();

            var isUnitNotAction = false;
            
            foreach (var unitEntity in unitsEntities)
            {
                ref var unitComponent = ref unitEntity.GetComponent<ArenaUnitComponent>();
                if (!unitComponent.IsActionCurrentRound)
                    isUnitNotAction = true;
            }
            if (!isUnitNotAction)
            {
                foreach (var unitEntity in unitsEntities)
                {
                    ref var unitComponent = ref unitEntity.GetComponent<ArenaUnitComponent>();
                    unitComponent.IsActionCurrentRound = false;
                }
            }
            
            var selectUnitGUID = "";
            var minIndexTurnOrder = 999;

            foreach (var unitEntity in unitsEntities)
            {
                var unitComponent = unitEntity.GetComponent<ArenaUnitComponent>();
                if (unitComponent.IndexTurnOrder < minIndexTurnOrder && !unitComponent.IsActionCurrentRound)
                {
                    minIndexTurnOrder = unitComponent.IndexTurnOrder;
                    selectUnitGUID = unitComponent.GUID;
                }
            }

            var selectUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.GUID == selectUnitGUID)
                .SelectFirstEntity();
            selectUnitEntity.AddComponent(new ArenaUnitCurrentComponent());

            SelectUnitVisual();
        }

        private void SelectUnitVisual()
        {
            var selectUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();

            var unitComponent = selectUnitEntity.GetComponent<ArenaUnitComponent>();
            unitComponent.UnitArenaMono.UnitPointVFXMono.EnableEffect();
        }

        private void FinishRound()
        {
            var isEndRound = ArenaAction.CheckFinishArenaBattle.Invoke();

            if (isEndRound)
            {
                ArenaAction.EndBattleArena?.Invoke();
                return;
            }
            
            ArenaAction.UpdateTurnOrderArena?.Invoke();
            UpdateRoundVisual();
        }

        private void EnableControlPlayer()
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerArenaInBattleComponent>();

            if (currentPlayerComponent.PlayerControlEntity == PlayerControlEntity.NeutralUnits)
            {
                ArenaAIAction.StartAINeutralLogic?.Invoke();
            }
            else
            {
                var playerGlobalEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == currentPlayerComponent.PlayerID)
                    .SelectFirstEntity();
                var playerGlobalComponent = playerGlobalEntity.GetComponent<PlayerComponent>();

                if (playerGlobalComponent.playerOrAI == PlayerOrAI.Player)
                {
                    ArenaUIAction.ShowHUDButton?.Invoke();
                }
                else
                {
                    AIBattleArenaAction.StartAIRound?.Invoke();
                }
            }
        }
        
        public void Destroy()
        {
            ArenaAction.UpdateRound -= UpdateRoundVisual;
            ArenaAction.FinishRound -= FinishRound;
        }
    }
}