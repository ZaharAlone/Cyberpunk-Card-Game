using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.AI;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using Object = UnityEngine.Object;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDestroyUnitSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            AbilityCardAction.DestroyNeutralUnit += AbilityDestroyNeutralUnit;
            AbilityCardAction.DestroyEnemyUnit += AbilityDestroyEnemyUnit;
        }
        
        private void AbilityDestroyNeutralUnit(string guidCard)
        {
            Debug.LogError("Ability Card Destroy Neutral Unit");
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.DestroyNeutralUnit?.Invoke(guidCard);
                return;
            }
            
            CityAction.ShowWhereZoneToPlayerID?.Invoke(-1);
            StartWorkAbility();
            CityAction.SelectTower += DestroyNeutralUnitInMap;
        }

        private void AbilityDestroyEnemyUnit(string guidCard)
        {
            Debug.LogError("Ability Card Destroy Enemy Unit");
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
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
            CityAction.SelectTower += DestroyEnemyUnitInMap;
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
            
            AbilitySelectElementUIAction.OpenSelectAbilityCard?.Invoke(AbilityType.AddUnit, 0, false);
            BezierCurveNavigationAction.StartBezierCurve?.Invoke(cardPosition, BezierTargetEnum.Tower);
        }

        private void DestroyNeutralUnitInMap(string towerGUID)
        {
            var unitEntity = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                    && unit.PowerSolidPlayerID == -1)
                .SelectFirstEntity();

            var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
            Object.Destroy(unitComponent.UnitIconsGO);
            unitEntity.Destroy();
            
            CityAction.SelectTower -= DestroyNeutralUnitInMap;
            FinishWorkAbility();
        }
        
        private void DestroyEnemyUnitInMap(string towerGUID)
        {
            var playerEntities = _dataWorld.Select<PlayerComponent>()
                .Without<CurrentPlayerComponent>()
                .GetEntities();

            foreach (var playerEntity in playerEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerComponent>();
                
                var isEntity = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.PowerSolidPlayerID == playerComponent.PlayerID)
                    .TrySelectFirstEntity(out var unitEntity);

                if (isEntity)
                {
                    var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                    Object.Destroy(unitComponent.UnitIconsGO);
                    unitEntity.Destroy();
                    
                    break;
                }   
            }
            
            CityAction.SelectTower -= DestroyEnemyUnitInMap;
            FinishWorkAbility();
        }
        
        private void FinishWorkAbility()
        {
            AbilityCardAction.CurrentAbilityEndPlaying?.Invoke();
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
        }

        public void Destroy()
        {
            AbilityCardAction.DestroyNeutralUnit -= AbilityDestroyNeutralUnit;
            AbilityCardAction.DestroyEnemyUnit -= AbilityDestroyEnemyUnit;
        }
    }
}