using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AI;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using Object = UnityEngine.Object;

namespace CyberNet.Core.AbilityCard
{
    /// <summary>
    /// Дописать абилку замены вражеского юнита, для этого нужно добавить выделение юнита
    /// </summary>
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
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.DestroyEnemyUnit?.Invoke(guidCard);
                return;
            }

            var playerEntities = _dataWorld.Select<PlayerComponent>()
                .Without<CurrentPlayerComponent>()
                .GetEntities();

            var listIDPlayers = new List<int>();
            
            foreach (var playerEntity in playerEntities)
            {
                listIDPlayers.Add(playerEntity.GetComponent<PlayerComponent>().PlayerID);
            }
            
            CityAction.ShowManyZonePlayerInMap?.Invoke(listIDPlayers);
            StartWorkAbility(guidCard);
            
            // Дописать выбор не башни,а конкретного юнита
            //CityAction.SelectTower += DestroyEnemyUnitInMap;
        }

        private void StartSwitchNeutralUnit(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.DestroyNeutralUnit?.Invoke(guidCard);
                return;
            }
            
            CityAction.ShowWhereZoneToPlayerID?.Invoke(-1);
            StartWorkAbility(guidCard);
            CityAction.SelectDistrict += SwitchNeutralUnitMapSelectTower;
        }

        private void StartWorkAbility(string guidCard)
        {
            var abilityType = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity()
                .GetComponent<AbilitySelectElementComponent>()
                .AbilityCard.AbilityType;
            
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(abilityType, 0, false);
            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Tower);
        }
        
        private void SwitchNeutralUnitMapSelectTower(string towerGUID)
        {
            var unitEntity = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == towerGUID
                    && unit.PowerSolidPlayerID == -1)
                .SelectFirstEntity();

            var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
            Object.Destroy(unitComponent.UnitIconsGO);
            unitEntity.Destroy();

            AbilityCardAction.AddTowerUnit?.Invoke(towerGUID);
            
            CityAction.SelectDistrict -= SwitchNeutralUnitMapSelectTower;
            FinishWorkAbility();
        }

        private void FinishWorkAbility()
        {
            AbilityCardAction.CurrentAbilityEndPlaying?.Invoke();
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
        }
        
        public void Destroy()
        {
            AbilityCardAction.SwitchEnemyUnitMap -= SwitchEnemyUnitMap;
            AbilityCardAction.SwitchNeutralUnitMap -= StartSwitchNeutralUnit;
            
            CityAction.SelectDistrict -= SwitchNeutralUnitMapSelectTower;
        }
    }
}