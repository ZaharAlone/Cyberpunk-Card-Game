using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AI;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Global;
using Object = UnityEngine.Object;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilitySwitchUnitMapSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.SwitchEnemyUnitMap += SwitchEnemyUnitMap;
            AbilityCardAction.SwitchNeutralUnitMap += StartSwitchNeutralUnit;
        }
        private void SwitchEnemyUnitMap(string guidCard)
        {
            Debug.LogError("Ability Card Enemy");
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                AbilityAIAction.DestroyEnemyUnit?.Invoke(guidCard);
                return;
            }

            var playerEntities = _dataWorld.Select<PlayerComponent>()
                .Without<CurrentPlayerComponent>()
                .GetEntities();

            var listID = new List<int>();
            
            foreach (var playerEntity in playerEntities)
            {
                listID.Add(playerEntity.GetComponent<PlayerComponent>().PlayerID);
            }
            
            CityAction.ShowManyZonePlayerInMap?.Invoke(listID);
            StartWorkAbility();
            //CityAction.SelectTower += DestroyEnemyUnitInMap;
        }

        private void StartSwitchNeutralUnit(string guidCard)
        {
            Debug.LogError("Ability Card Switch neutral");
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                AbilityAIAction.DestroyNeutralUnit?.Invoke(guidCard);
                return;
            }
            
            CityAction.ShowWhereZoneToPlayerID?.Invoke(-1);
            StartWorkAbility();
            CityAction.SelectTower += SwitchNeutralUnitMapSelectTower;
        }
        
        private void SwitchNeutralUnitMapSelectTower(string towerGUID)
        {
            var unitEntity = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                    && unit.PowerSolidPlayerID == -1)
                .SelectFirstEntity();

            var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
            Object.Destroy(unitComponent.UnitIconsGO);
            unitEntity.Destroy();

            AbilityCardAction.AddTowerUnit?.Invoke(towerGUID);
            
            CityAction.SelectTower -= SwitchNeutralUnitMapSelectTower;
            FinishWorkAbility();
        }
        
        private void FinishWorkAbility()
        {
            AbilityCardAction.CurrentAbilityEndPlaying?.Invoke();
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
        }

        private void StartWorkAbility()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = true;
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();

            var cardComponent = entityCard.GetComponent<CardComponent>();
            var cardPosition = cardComponent.RectTransform.position;
            cardPosition.y += cardComponent.RectTransform.sizeDelta.y / 2;
            
            AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.Attack, 0, false);
            BezierCurveNavigationAction.StartBezierCurve?.Invoke(cardPosition, BezierTargetEnum.Tower);
        }
        
        public void Destroy()
        {
            AbilityCardAction.SwitchEnemyUnitMap -= SwitchEnemyUnitMap;
            AbilityCardAction.SwitchNeutralUnitMap -= StartSwitchNeutralUnit;
        }
    }
}