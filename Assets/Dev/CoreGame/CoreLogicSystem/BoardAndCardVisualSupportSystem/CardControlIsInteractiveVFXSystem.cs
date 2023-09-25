using CyberNet.Core.AbilityCard;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
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
    public class CardControlIsInteractiveVFXSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            VFXCardInteractivAction.UpdateVFXCard += UpdateVFXViewCurrentPlayer;
        }

        private void UpdateVFXViewCurrentPlayer()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.PlayerType == PlayerType.Player)
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
                                               .With<CardTableComponent>()
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
                ref var component = ref entity.GetComponent<CardComponent>().CardMono;
                component.SetStatusInteractiveVFX(true);
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
    }
}