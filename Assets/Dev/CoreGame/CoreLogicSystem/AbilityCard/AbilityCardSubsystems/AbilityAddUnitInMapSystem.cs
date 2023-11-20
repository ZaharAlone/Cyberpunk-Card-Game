using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.AI;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityAddUnitInMapSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

         public void PreInit()
        {
            AbilityCardAction.AddUnitMap += AddUnitMap;
        }
        
        private void AddUnitMap()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                AbilityAIAction.AddUnitMap?.Invoke();
                return;
            }
            
            roundData.PauseInteractive = true;
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();

            var cardComponent = entityCard.GetComponent<CardComponent>();
            var cardPosition = cardComponent.RectTransform.position;
            cardPosition.y += cardComponent.RectTransform.sizeDelta.y / 2;
            
            CityAction.ShowWherePlayerCanAddUnit?.Invoke();
            //AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.Attack, 0, false);
            BezierCurveNavigationAction.StartBezierCurve?.Invoke(cardPosition, BezierTargetEnum.Tower);
            CityAction.SelectTower += AddUnitTower;
        }
        
        private void AddUnitTower(string towerGUID)
        {
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
            
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref var playerVisualComponent = ref playerEntity.GetComponent<PlayerViewComponent>();
            playerComponent.UnitCount--;
            var playerID = playerComponent.PlayerID;
            
            //TODO ГД вопрос, подумать и возможно переписать на только одну фракцию на территории
            /*
            var targetSquadZone = 0;
            foreach (SquadZoneMono squadZone in towerComponent.SquadZonesMono)
            {
                bool isClose = _dataWorld.Select<SquadMapComponent>()
                    .Where<SquadMapComponent>(unit => unit.GUIDPoint == towerGUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID != playerID)
                    .TrySelectFirstEntity(out Entity t);

                if (isClose)
                    targetSquadZone = squadZone.Index + 1;
                else
                {
                    targetSquadZone = squadZone.Index;
                    break;
                }
            }
            */
            var initUnit = new InitUnitStruct {
                KeyUnit = playerVisualComponent.KeyCityVisual,
                SquadZone = towerComponent.TowerMono.SquadZonesMono[0],
                PlayerControl = PlayerControlEnum.Player, TargetPlayerID = playerID,
            };

            CityAction.InitUnit?.Invoke(initUnit);
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI?.Invoke();

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
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = false;
            
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .With<AbilityCardAddUnitComponent>()
                .SelectFirstEntity();

            entityCard.RemoveComponent<AbilitySelectElementComponent>();
            entityCard.RemoveComponent<AbilityCardAddUnitComponent>();
            entityCard.RemoveComponent<SelectTargetCardAbilityComponent>();
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            
            entityCard.AddComponent(new CardMoveToTableComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
            AbilityCancelButtonUIAction.HideCancelButton?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
            CityAction.SelectTower -= AddUnitTower;
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
        }
    }
}