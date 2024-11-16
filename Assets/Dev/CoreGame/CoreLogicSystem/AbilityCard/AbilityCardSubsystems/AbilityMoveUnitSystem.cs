using CyberNet.Core.AI.Ability;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Map;
using CyberNet.Core.Map.InteractiveElement;
using CyberNet.Core.Map.InteractiveElement.Support;
using CyberNet.Core.Player;
using CyberNet.Global;
using CyberNet.Global.Cursor;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.MapMoveUnit;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityMoveUnitSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            AbilityCardAction.MoveUnit += MoveUnit;
            AbilityCardAction.CancelMoveUnit += CancelMoveUnit;
        }

        private void MoveUnit(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.MoveUnit?.Invoke(guidCard);
                return;
            }

            _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity()
                .AddComponent(new AbilityCardMoveUnitInProgressComponent());
            
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(AbilityType.UnitMove, 0, false);
            PreSelectTower(guidCard);
        }
        
        private void PreSelectTower(string guidCard)
        {
            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Tower);
            CityAction.ShowWherePlayerCanMove?.Invoke();

            _dataWorld.NewEntity().AddComponent(new FollowClickDistrictComponent());
            CityAction.SelectDistrict += SelectTower;
        }
        
        private void SelectTower(string towerGUID)
        {
            CityAction.SelectDistrict -= SelectTower;
            _dataWorld.Select<FollowClickDistrictComponent>().SelectFirstEntity().Destroy();
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();

            var entityMoveCard = _dataWorld.Select<MoveUnitComponent>().SelectFirstEntity();
            ref var moveCardComponent = ref entityMoveCard.GetComponent<MoveUnitComponent>();
            moveCardComponent.SelectDistrictGUID = towerGUID;

            entityMoveCard.AddComponent(new MoveUnitSelectTowerComponent());
            CityAction.UpdateCanInteractiveMap?.Invoke();

            FollowSelectUnitToMove();
        }

        private void FollowSelectUnitToMove()
        {
            var canMoveUnitComponent = _dataWorld.Select<MoveUnitComponent>().SelectFirstEntity()
                .GetComponent<MoveUnitComponent>();

            var currentPlayerID = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity()
                .GetComponent<PlayerComponent>()
                .PlayerID;
            
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(AbilityType.UnitMove, 1, false);
            var followDistrictConfig = new TargetDistrictAndPlayerIDDTO {
                GUIDDistrict = canMoveUnitComponent.SelectDistrictGUID, TargetPlayerID = currentPlayerID
            };
            FollowSelectInteractiveMapAction.StartFollowSelectUnit?.Invoke(followDistrictConfig);

            FollowSelectInteractiveMapAction.UpdateSelectUnit += CheckUpdateReadinessUnitsForShipment;
        }
        
        //Проверяем могут ли быть отправлены отряды сейчас
        private void CheckUpdateReadinessUnitsForShipment()
        {
            var countSelectUnit = _dataWorld.Select<SelectUnitMapComponent>()
                .Count();

            var entityMoveCard = _dataWorld.Select<MoveUnitComponent>().SelectFirstEntity();
            var targetTowerGUID = entityMoveCard.GetComponent<MoveUnitComponent>().SelectDistrictGUID;
            
            if (countSelectUnit > 0)
                CityAction.EnableInteractiveTower?.Invoke(targetTowerGUID);
            else
                CityAction.DisableInteractiveTower?.Invoke(targetTowerGUID);
        }

        private void CancelMoveUnit(string guidCard)
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();

            //Убираем выделение с юнитов если какие-то юниты уже выделены
            var selectUnitsMapEntities = _dataWorld.Select<SelectUnitMapComponent>().GetEntities();

            foreach (var unitEntity in selectUnitsMapEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                unitEntity.RemoveComponent<SelectUnitMapComponent>();
                unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();   
            }
            
            entityCard.RemoveComponent<MoveUnitComponent>();
            
            if (entityCard.HasComponent<SelectTargetCardAbilityComponent>())
                entityCard.RemoveComponent<SelectTargetCardAbilityComponent>();
            
            if (entityCard.HasComponent<MoveUnitSelectTowerComponent>())
                entityCard.RemoveComponent<MoveUnitSelectTowerComponent>();
            
            CityAction.DeactivateAllTower?.Invoke();
            CityAction.SelectDistrict -= SelectTower;
            CityAction.SelectUnit -= ClickOnUnit;
            CityAction.SelectDistrict -= SelectDistrictToMove;
            CustomCursorAction.OnBaseCursor?.Invoke();
        }

        public void Destroy()
        {
            AbilityCardAction.MoveUnit -= MoveUnit;
            AbilityCardAction.CancelMoveUnit -= CancelMoveUnit;
        }
    }
}