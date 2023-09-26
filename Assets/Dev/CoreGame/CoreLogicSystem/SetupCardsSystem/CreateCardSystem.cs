using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard;
using Object = UnityEngine.Object;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class CreateCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SetupCardAction.InitCard += InitCard;
        }
        
        private Entity InitCard(CardData placeCard, Transform parent, bool isPlayerCard)
        {
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardMono = Object.Instantiate(boardGameConfig.CardGO, parent);
            var cardGO = cardMono.gameObject;
            cardsConfig.Cards.TryGetValue(placeCard.CardName, out var card);

            SetViewCard(cardMono, card);

            var entity = _dataWorld.NewEntity();
            var cardComponent = SetCardComponent.Set(cardGO, card, cardMono);
            
            if (isPlayerCard)
            {
                cardGO.transform.localScale = boardGameConfig.SizeCardInDeck;
            }
            else // card is trade row
            {
                cardComponent.PlayerID = -1;
                cardGO.transform.localScale = boardGameConfig.SizeCardInTraderow;
            }
            
            entity.AddComponent(cardComponent);
            entity.AddComponent(new CardSortingIndexComponent { Index = placeCard.IDPositions });
            return entity;
        }
        
        private void SetViewCard(CardMono card, CardConfigJson cardConfigJson)
        {
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardsImage = _dataWorld.OneData<BoardGameData>().CardsImage;
            cardsImage.TryGetValue(cardConfigJson.ImageKey, out var cardImage);

            if (cardImage == null)
            {
                Debug.LogError($"Not find card image is name: {cardConfigJson.ImageKey}");
                return;
            }

            if (cardConfigJson.Nations != "Neutral")
            {
                boardGameConfig.NationsImage.TryGetValue(cardConfigJson.Nations, out var nationsImage);
                card.SetViewCard(cardImage, cardConfigJson.Header, cardConfigJson.CyberpsychosisCount, cardConfigJson.Price, nationsImage);

                for (int i = 0; i < cardConfigJson.Count; i++)
                    Object.Instantiate(boardGameConfig.ItemIconsCounterCard, card.CountCardBlock);
            }
            else
                card.SetViewCard(cardImage, cardConfigJson.Header, cardConfigJson.CyberpsychosisCount, cardConfigJson.Price);

            var isAbility_0 = cardConfigJson.Ability_0.AbilityType != AbilityType.None;
            var isAbility_1 = cardConfigJson.Ability_1.AbilityType != AbilityType.None;
            var isAbility_2 = cardConfigJson.Ability_2.AbilityType != AbilityType.None;
            var onlyOneAbility = isAbility_0 && !isAbility_1 && !isAbility_2;
            var chooseAbility = isAbility_0 && isAbility_1;

            card.SetChooseAbility(chooseAbility);
            card.IsConditionAbility(isAbility_2);
            
            if (!chooseAbility)
                card.AbilityBlock_2_Container.gameObject.SetActive(false);
            
            if (chooseAbility && isAbility_2)
                card.SetBigDownBlock();
            
            if (cardConfigJson.Ability_0.AbilityType != AbilityType.None)
            {
                if (onlyOneAbility)
                    SetViewAbilityCard.SetView(card.AbilityBlock_OneShot_Container, cardConfigJson.Ability_0, boardGameConfig, chooseAbility, onlyOneAbility);
                else
                    SetViewAbilityCard.SetView(card.AbilityBlock_1_Container, cardConfigJson.Ability_0, boardGameConfig, chooseAbility);
            }
            if (cardConfigJson.Ability_1.AbilityType != AbilityType.None)
                SetViewAbilityCard.SetView(card.AbilityBlock_2_Container, cardConfigJson.Ability_1, boardGameConfig, chooseAbility);
            if (cardConfigJson.Ability_2.AbilityType != AbilityType.None)
                SetViewAbilityCard.SetView(card.AbilityBlock_3_Container, cardConfigJson.Ability_2, boardGameConfig);

            card.CardOnBack();
        }
    }
}