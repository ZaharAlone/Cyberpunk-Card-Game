using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.Map;
using CyberNet.Core.Map.InteractiveElement;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityAddUnitInMapSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

         public void PreInit()
        {
            AbilityCardAction.AbilityAddUnitMap += AddUnitMap;
        }

        private void AddUnitMap(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.AddUnitMap?.Invoke(guidCard);
                return;
            }

            var cardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();
            cardEntity.AddComponent(new FollowClickDistrictComponent());

            CityAction.ShowWhereZoneToPlayerID?.Invoke(roundData.CurrentPlayerID);
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(AbilityType.AddUnit, 0, false);
            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Tower);

            CityAction.SelectDistrict += AddUnitTower;
            AbilityCardAction.CancelAddUnitMap += CancelAddUnitMap;
        }

        private void AddUnitTower(string towerGUID)
        {
            AbilityCardAction.AddTowerUnit?.Invoke(towerGUID);

            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .With<AbilityCardAddUnitComponent>()
                .SelectFirstEntity();
            ref var abilityAddComponentComponent = ref entityCard.GetComponent<AbilityCardAddUnitComponent>();
            abilityAddComponentComponent.CountUseElement++;
            abilityAddComponentComponent.ListTowerAddUnit.Add(towerGUID);
            
            CheckEndActionAbility();
        }

        private void CheckEndActionAbility()
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .With<AbilityCardAddUnitComponent>()
                .SelectFirstEntity();
            
            var abilitySelectElementComponent = entityCard.GetComponent<AbilitySelectElementComponent>();
            var abilityAddComponentComponent = entityCard.GetComponent<AbilityCardAddUnitComponent>();
            
            if (abilitySelectElementComponent.AbilityCard.Count == abilityAddComponentComponent.CountUseElement)
                EndActionAbility();
        }

        private void EndActionAbility()
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .With<AbilityCardAddUnitComponent>()
                .SelectFirstEntity();

            var cardComponent = entityCard.GetComponent<CardComponent>();
            entityCard.RemoveComponent<AbilityCardAddUnitComponent>();
            entityCard.RemoveComponent<FollowClickDistrictComponent>();

            AbilityCardAction.CompletePlayingAbilityCard?.Invoke(cardComponent.GUID);
            CityAction.UpdateCanInteractiveMap?.Invoke();
            CityAction.SelectDistrict -= AddUnitTower;
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
            
            AbilityCardAction.CancelAddUnitMap -= CancelAddUnitMap;
        }
        
        private void CancelAddUnitMap(string guidCard)
        {
            AbilityCardAction.CancelAddUnitMap -= CancelAddUnitMap;
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();
            
            //TODO проверить робоспособность
            //Учитывать что можно добавить больше одного юнита за раз и их нужно всех откатить

            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var unitsAddedToMapComponent = entityCard.GetComponent<AbilityCardAddUnitComponent>();
            
            foreach (var newUnitsInTower in unitsAddedToMapComponent.ListTowerAddUnit)
            {
                var unitInMapEntity = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDDistrict == newUnitsInTower
                        && unit.PowerSolidPlayerID == currentPlayerID)
                    .SelectFirstEntity();
                var unitInMapComponent = unitInMapEntity.GetComponent<UnitMapComponent>();
                
                Object.Destroy(unitInMapComponent.UnitIconsGO);
                unitInMapEntity.Destroy();
            }
            
            entityCard.RemoveComponent<AbilityCardAddUnitComponent>();
            entityCard.RemoveComponent<FollowClickDistrictComponent>();
            
            CityAction.SelectDistrict -= AddUnitTower;
        }

        public void Destroy()
        {
            AbilityCardAction.AbilityAddUnitMap -= AddUnitMap;
            AbilityCardAction.CancelAddUnitMap -= CancelAddUnitMap;
            CityAction.SelectDistrict -= AddUnitTower;
        }
    }
}