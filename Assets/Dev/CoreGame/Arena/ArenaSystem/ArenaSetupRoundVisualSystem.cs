using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaSetupRoundVisualSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        /* Старт хода
         * 3) Даем управление игроку, игрок может выбрать как цель юнита противника и атаковать его
         * 4) ... разыгрыш карт пока убираем, но нужно дописать логику которая убирает выделение с карт,
         *        которые нельзя играть на арене
         * 5) Игрок может выбрать как цель противника, и закончить ход
         * 6) Игрок может спасовать - пока не делаем
         *
         * 7) Описываем поведение нейтрального юнита
         * 8) Создаем цикл с перестрелкой и захватом территории
         */

        public void PreInit()
        {
            ArenaAction.UpdateRound += UpdateRoundVisual;
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
            
            FindPlayerInCurrentRound();
            SwitchRoundCamera();
            SelectCurrentUnitVisual();
        }

        private void FindPlayerInCurrentRound()
        {
            ref var roundData = ref _dataWorld.OneData<ArenaRoundData>();
            var playersInBattleEntities = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .GetEntities();

            var positionInTurnQueue = 50;
            foreach (var playerEntity in playersInBattleEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerArenaInBattleComponent>();
                if (playerComponent.PositionInTurnQueue < positionInTurnQueue)
                {
                    roundData.PlayerControlEnum = playerComponent.PlayerControlEnum;
                    roundData.CurrentPlayerID = playerComponent.PositionInTurnQueue;
                }
            }
        }

        private void SwitchRoundCamera()
        {
            var roundData = _dataWorld.OneData<ArenaRoundData>();
            var playersInBattleEntity = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Where<PlayerArenaInBattleComponent>(player => player.PlayerID == roundData.CurrentPlayerID 
                    && player.PlayerControlEnum == roundData.PlayerControlEnum)
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
                    && unit.PlayerControlEnum == roundData.PlayerControlEnum)
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
        
        public void Destroy()
        {
            ArenaAction.UpdateRound -= UpdateRoundVisual;
        }
    }
}