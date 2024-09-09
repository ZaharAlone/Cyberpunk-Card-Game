using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaRoundSystem : IPreInitSystem, IInitSystem, IDestroySystem
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
            ArenaAction.DeselectPlayer?.Invoke();
            
            // Ищем следующего игрока, и включаем весь его визуал
            ArenaAction.FindPlayerInCurrentRound();
            SwitchRoundCamera();
            SelectCurrentUnitVisual();

            // Выдаем контроль игроку, или AI
            ArenaAction.UpdatePlayerInputsRound?.Invoke();
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
                    && unit.PlayerControlEntity == roundData.PlayerControlEntity)
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
            var colorsConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ColorsGameConfigSO;
            unitComponent.UnitArenaMono.UnitPointVFXMono.EnableEffect();
            unitComponent.UnitArenaMono.UnitPointVFXMono.SetColor(colorsConfig.SelectCurrentTargetBlueColor, false);
        }

        private void FinishRound()
        {
            var isEndRound = ArenaAction.CheckFinishArenaBattle.Invoke();

            if (isEndRound)
            {
                ArenaAction.EndBattleArena?.Invoke();
                return;
            }

            var selectTargetForAttack = _dataWorld.Select<ArenaSelectUnitForAttackComponent>().GetEntities();
            foreach (var entity in selectTargetForAttack)
            {
                entity.RemoveComponent<ArenaSelectUnitForAttackComponent>();
            }
            
            ArenaAction.UpdateTurnOrderArena?.Invoke();
            UpdateRoundVisual();
        }
        
        public void Destroy()
        {
            ArenaAction.UpdateRound -= UpdateRoundVisual;
            ArenaAction.FinishRound -= FinishRound;
            
            _dataWorld.RemoveOneData<ArenaRoundData>();
        }
    }
}