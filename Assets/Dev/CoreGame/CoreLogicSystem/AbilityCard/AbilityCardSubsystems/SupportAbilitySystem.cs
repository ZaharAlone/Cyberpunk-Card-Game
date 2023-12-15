using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class SupportAbilitySystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.AddTowerUnit += AddTowerUnit;
            AbilityCardAction.CurrentAbilityEndPlaying += CurrentAbilityEndPlaying;
        }
        private void AddTowerUnit(string towerGUID)
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
            
            var targetSquadZone = FindFreeSlotInTower(towerComponent, playerID);
            
            var initUnit = new InitUnitStruct {
                KeyUnit = playerVisualComponent.KeyCityVisual,
                UnitZone = towerComponent.TowerMono.SquadZonesMono[targetSquadZone],
                PlayerControl = PlayerControlEnum.Player, TargetPlayerID = playerID,
            };

            CityAction.InitUnit?.Invoke(initUnit);
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI?.Invoke();
        }

        private int FindFreeSlotInTower(TowerComponent towerComponent, int playerID)
        {
            var targetSquadZone = 0;
            foreach (var squadZone in towerComponent.TowerMono.SquadZonesMono)
            {
                var isTargetSlot = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerComponent.GUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID == playerID)
                    .Count() > 0;

                if (isTargetSlot)
                    break;
                else
                {
                    targetSquadZone = squadZone.Index + 1;
                }
            }

            if (targetSquadZone > towerComponent.TowerMono.SquadZonesMono.Count - 1)
            {
                targetSquadZone = 0;
                foreach (var squadZone in towerComponent.TowerMono.SquadZonesMono)
                {
                    var isTargetSlot = _dataWorld.Select<UnitMapComponent>()
                        .Where<UnitMapComponent>(unit => unit.GUIDTower == towerComponent.GUID
                            && unit.IndexPoint == squadZone.Index)
                        .Count() == 0;

                    if (isTargetSlot)
                        break;
                    else
                    {
                        targetSquadZone = squadZone.Index + 1;
                    }
                }
            }
            
            return targetSquadZone;
        }
        
        private void CurrentAbilityEndPlaying()
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = false;
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();
            
            entityCard.RemoveComponent<AbilitySelectElementComponent>();
            entityCard.RemoveComponent<SelectTargetCardAbilityComponent>();
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            
            entityCard.AddComponent(new CardMoveToTableComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
            AbilitySelectElementAction.ClosePopup?.Invoke();
            AbilityCancelButtonUIAction.HideCancelButton?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
        }
        
        public void Destroy()
        {
            AbilityCardAction.AddTowerUnit -= AddTowerUnit;
            AbilityCardAction.CurrentAbilityEndPlaying -= CurrentAbilityEndPlaying;
        }
    }
}