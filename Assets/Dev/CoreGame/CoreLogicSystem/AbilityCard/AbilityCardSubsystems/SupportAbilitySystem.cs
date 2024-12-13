using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.Map;
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
            AbilityCardAction.ShiftUpCard += ShiftUpCard;
        }

        private void AddTowerUnit(string towerGUID)
        {
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            ref var towerComponent = ref towerEntity.GetComponent<DistrictComponent>();
            
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
                UnitZone = towerComponent.DistrictMono.SquadZonesMono[targetSquadZone],
                PlayerControl = PlayerControlEntity.PlayerControl, TargetPlayerID = playerID,
            };

            CityAction.InitUnit?.Invoke(initUnit);
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI?.Invoke();
        }

        private int FindFreeSlotInTower(DistrictComponent DistrictComponent, int playerID)
        {
            var targetSquadZone = 0;
            foreach (var squadZone in DistrictComponent.DistrictMono.SquadZonesMono)
            {
                var isTargetSlot = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDDistrict == DistrictComponent.GUID
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

            if (targetSquadZone > DistrictComponent.DistrictMono.SquadZonesMono.Count - 1)
            {
                targetSquadZone = 0;
                foreach (var squadZone in DistrictComponent.DistrictMono.SquadZonesMono)
                {
                    var isTargetSlot = _dataWorld.Select<UnitMapComponent>()
                        .Where<UnitMapComponent>(unit => unit.GUIDDistrict == DistrictComponent.GUID
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
            
            entityCard.AddComponent(new CardMoveToDiscardComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            
            AbilityPopupUISystemAction.ClosePopup?.Invoke();
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
        }

        private void ShiftUpCard(string guidCard)
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();
            
            var cardComponent = entityCard.GetComponent<CardComponent>();
            var cardPosition = cardComponent.RectTransform.position;
            cardPosition.y += cardComponent.RectTransform.sizeDelta.y / 2;
        }
        
        public void Destroy()
        {
            AbilityCardAction.AddTowerUnit -= AddTowerUnit;
            AbilityCardAction.CurrentAbilityEndPlaying -= CurrentAbilityEndPlaying;
            AbilityCardAction.ShiftUpCard -= ShiftUpCard;
        }
    }
}