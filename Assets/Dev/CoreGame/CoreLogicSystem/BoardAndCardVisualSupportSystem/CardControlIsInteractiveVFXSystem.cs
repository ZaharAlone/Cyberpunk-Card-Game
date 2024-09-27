using CyberNet.Core.AbilityCard;
using CyberNet.Core.Arena;
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
                cardMono.CardFaceMono.SetStatusInteractiveVFX(true);
            }
        }

        private void UpdateVFXViewCurrentPlayer()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            var currentPlayerID = roundData.CurrentPlayerID;

            var disableAllVFX = roundData.PauseInteractive || roundData.playerOrAI != PlayerOrAI.Player;

            //TODO поправить арена
            /*
            if (roundData.CurrentGameStateMapVSArena == GameStateMapVSArena.Arena)
            {
                var arenaRoundData = _dataWorld.OneData<ArenaRoundData>();
                currentPlayerID = arenaRoundData.CurrentPlayerID;

                var currentPlayerIsControlHuman = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == currentPlayerID)
                    .SelectFirstEntity()
                    .GetComponent<PlayerComponent>().playerOrAI == PlayerOrAI.Player;
                
                disableAllVFX = !currentPlayerIsControlHuman;
            }*/
            
            if (disableAllVFX)
            {
                DisableAllVFX();
            }
            else
            {
                UpdateVFX(currentPlayerID);
            }
        }

        private void DisableAllVFX()
        {
            var entitiesCard = _dataWorld.Select<CardComponent>()
                .GetEntities();
            
            foreach (var entity in entitiesCard)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                var cardMono = cardComponent.CardMono;

                cardMono.CardFaceMono.SetStatusInteractiveVFX(false);
                    
                if(entity.HasComponent<CardCanUseComponent>())
                    entity.RemoveComponent<CardCanUseComponent>();
            }
        }

        private void UpdateVFX(int playerID)
        {
            var isInstallFirstBase = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .With<PlayerNotInstallFirstBaseComponent>()
                .Count() == 0;
            
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
                var countAbilitiesAvailable = AbilityCardUtilsAction.CalculateHowManyAbilitiesAvailableForSelection.Invoke(cardComponent);
                
                if (countAbilitiesAvailable > 0 && isInstallFirstBase)
                {
                    cardMono.CardFaceMono.SetStatusInteractiveVFX(true);
                    entity.AddComponent(new CardCanUseComponent());
                }
                else
                {
                    cardMono.CardFaceMono.SetStatusInteractiveVFX(false);
                    
                    if(entity.HasComponent<CardCanUseComponent>())
                        entity.RemoveComponent<CardCanUseComponent>();
                }
            }

            foreach (var entity in entitiesCardInDeck)
            {
                ref var cardFace = ref entity.GetComponent<CardComponent>().CardMono.CardFaceMono;
                cardFace.SetStatusInteractiveVFX(false);
            }

            foreach (var entity in entitiesCardInDrop)
            {
                ref var cardFace = ref entity.GetComponent<CardComponent>().CardMono.CardFaceMono;
                cardFace.SetStatusInteractiveVFX(false);
            }
            
            foreach (var entity in entitiesCardInDropMove)
            {
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.CardFaceMono.SetStatusInteractiveVFX(false);
            }

            foreach (var entity in entitiesCardInShop)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                if (component.Price <= valueTrade && isInstallFirstBase)
                    component.CardMono.CardFaceMono.SetStatusInteractiveVFX(true);
                else
                    component.CardMono.CardFaceMono.SetStatusInteractiveVFX(false);
            }
        }

        public void Destroy()
        {
            VFXCardInteractiveAction.UpdateVFXCard -= UpdateVFXViewCurrentPlayer;
            VFXCardInteractiveAction.EnableVFXAllCardInHand -= EnableVFXAllCardInHand;
        }
    }
}