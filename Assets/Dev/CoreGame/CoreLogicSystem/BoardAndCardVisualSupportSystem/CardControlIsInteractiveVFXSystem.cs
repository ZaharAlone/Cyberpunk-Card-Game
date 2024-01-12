using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.UI
{
    /// <summary>
    /// VFX вокруг кард - являющийся индикацией что с данной картой можно взаимодействовать,
    /// к примеру с картой в руке или на рынке
    /// Вызывается:
    /// При выкладывание карты на стол
    /// При пополнение руки игрока
    /// При покупке карт
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class CardControlIsInteractiveVFXSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            VFXCardInteractiveAction.UpdateVFXCard += UpdateVFXViewCurrentPlayer;
            VFXCardInteractiveAction.EnableVFXAllCardInHand += EnableVFXAllCardInHand;
        }

        private void EnableVFXAllCardInHand()
        {
            var playerID = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity()
                .GetComponent<PlayerComponent>().PlayerID;
            var entitiesCardInHand = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerID)
                .With<CardHandComponent>()
                .GetEntities();
            
            foreach (var entity in entitiesCardInHand)
            {
                var cardMono = entity.GetComponent<CardComponent>().CardMono;
                cardMono.SetStatusInteractiveVFX(true);
            }
        }

        private void UpdateVFXViewCurrentPlayer()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.playerOrAI == PlayerOrAI.Player)
                UpdateVFX(roundData.CurrentPlayerID);
        }

        private void UpdateVFX(int playerID)
        {
            var entitiesCardInHand = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.PlayerID == playerID)
                                               .With<CardHandComponent>()
                                               .GetEntities();
            var entitiesCardInDeck = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.PlayerID == playerID)
                                               .With<CardAbilitySelectionCompletedComponent>()
                                               .GetEntities();
            var entitiesCardInDrop = _dataWorld.Select<CardComponent>()
                                               .Where<CardComponent>(card => card.PlayerID == playerID)
                                               .With<CardDiscardComponent>()
                                               .GetEntities();
            
            var entitiesCardInDropMove = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerID)
                .With<CardMoveToDiscardComponent>()
                .GetEntities();
            
            var entitiesCardInShop = _dataWorld.Select<CardComponent>().With<CardTradeRowComponent>().GetEntities();
            var actionValue = _dataWorld.OneData<ActionCardData>();
            var valueTrade = actionValue.TotalTrade - actionValue.SpendTrade;

            foreach (var entity in entitiesCardInHand)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                var cardMono = cardComponent.CardMono;

                if (CheckAbilityCardToShowCard(cardComponent))
                {
                    cardMono.SetStatusInteractiveVFX(true);
                    entity.AddComponent(new CardCanUseComponent());
                }
                else
                {
                    cardMono.SetStatusInteractiveVFX(false);
                    
                    if(entity.HasComponent<CardCanUseComponent>())
                        entity.RemoveComponent<CardCanUseComponent>();
                }
            }

            foreach (var entity in entitiesCardInDeck)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }

            foreach (var entity in entitiesCardInDrop)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }
            
            foreach (var entity in entitiesCardInDropMove)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(false);
            }

            foreach (var entity in entitiesCardInShop)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                if (component.Price <= valueTrade)
                    component.CardMono.SetStatusInteractiveVFX(true);
                else
                    component.CardMono.SetStatusInteractiveVFX(false);
            }
        }
        private bool CheckAbilityCardToShowCard(CardComponent cardComponent)
        {
            var showCard = false;

            if (cardComponent.Ability_0.AbilityType != AbilityType.None)
            {
                showCard = CheckAbilityCard(cardComponent.Ability_0.AbilityType);
            }
            
            if (cardComponent.Ability_1.AbilityType != AbilityType.None)
            {
                showCard = CheckAbilityCard(cardComponent.Ability_1.AbilityType);
            }
            
            if (cardComponent.Ability_2.AbilityType != AbilityType.None)
            {
                showCard = CheckAbilityCard(cardComponent.Ability_2.AbilityType);
            }
            
            return showCard;
        }

        private bool CheckAbilityCard(AbilityType abilityType)
        {
            var currentRoundState = _dataWorld.OneData<RoundData>().CurrentRoundState;
            var abilityCardConfig = _dataWorld.OneData<CardsConfig>().AbilityCard;
            abilityCardConfig.TryGetValue(abilityType.ToString(), out var configCard);
            
            var isShow = false;
            if (currentRoundState == RoundState.Map)
            {
                if (configCard.VisualPlayingCardMap != VisualPlayingCardType.None)
                    isShow = true;
            }
            else
            {
                if (configCard.VisualPlayingCardArena != VisualPlayingCardType.None)
                    isShow = true;
            }
            return isShow;
        }

        public void Destroy()
        {
            VFXCardInteractiveAction.UpdateVFXCard -= UpdateVFXViewCurrentPlayer;
            VFXCardInteractiveAction.EnableVFXAllCardInHand -= EnableVFXAllCardInHand;
        }
    }
}