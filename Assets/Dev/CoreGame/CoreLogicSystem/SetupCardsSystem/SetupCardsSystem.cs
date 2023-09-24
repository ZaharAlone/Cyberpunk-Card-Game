using CyberNet.Core.AbilityCard;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using UnityEngine;
using CyberNet.Core.UI;
using CyberNet.Global;
using Unity.VisualScripting;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class SetupCardsSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SetupCard();
            SetPositionCard();
        }
        
        //Инициализируем все карты
        private void SetupCard()
        {
            var deckCardsData = _dataWorld.OneData<DeckCardsData>();
            var cardsParent = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            var traderowParent = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono.TraderowContainerForCard;
            
            foreach (var item in deckCardsData.NeutralShopCards)
            {
                var entity = InitCard(item, traderowParent, false);
                entity.AddComponent(new CardNeutralComponent());
                entity.AddComponent(new CardTradeRowComponent());
            }
            
            foreach (var item in deckCardsData.ShopCards)
            {
                var entity = InitCard(item, traderowParent, false);
                entity.AddComponent(new CardTradeDeckComponent());
            }

            foreach (var playerDeckCard in deckCardsData.PlayerDeckCard)
            {
                foreach (var item in playerDeckCard.Cards)
                {
                    var entity = InitCard(item, cardsParent, true);
                    ref var cardComponent = ref entity.GetComponent<CardComponent>();
                    cardComponent.PlayerID = playerDeckCard.IndexPlayer;
                    entity.AddComponent(new CardDrawComponent());
                    entity.AddComponent(new CardPlayerComponent());
                }
            }
        }

        private Entity InitCard(CardData placeCard, Transform parent, bool isPlayerCard)
        {
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardMono = Object.Instantiate(boardGameConfig.CardGO, parent);
            var cardGO = cardMono.gameObject;
            cardsConfig.Cards.TryGetValue(placeCard.CardName, out var card);

            if (isPlayerCard)
                cardGO.transform.localScale = boardGameConfig.SizeCardInDeck;
            else // card is trade row
                cardGO.transform.localScale = boardGameConfig.SizeCardInTraderow;

            SetViewCard(cardMono, card);

            var entity = _dataWorld.NewEntity();
            var cardComponent = SetCardComponent.Set(cardGO, card, cardMono);
            entity.AddComponent(cardComponent);
            entity.AddComponent(new CardSortingIndexComponent { Index = placeCard.IDPositions });
            return entity;
        }

        //Отрисовываем вьюху карт
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
                card.AbilityBlock_2_Container.GameObject().SetActive(false);
            
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

        //Раскладываем карты по местам
        private void SetPositionCard()
        {
            var gameUI = _dataWorld.OneData<CoreGameUIData>();
            var entitiesPlayerCard = _dataWorld.Select<CardComponent>()
                .With<CardPlayerComponent>()
                .GetEntities();


            foreach (var entity in entitiesPlayerCard)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                component.RectTransform.position = gameUI.BoardGameUIMono.CoreHudUIMono.DownDeck.localPosition;
                component.CardMono.HideCard();
                component.CardMono.HideBackCardColor();
            }

            var entitiesDeck = _dataWorld.Select<CardComponent>().With<CardTradeDeckComponent>().GetEntities();
            foreach (var entity in entitiesDeck)
                entity.GetComponent<CardComponent>().CardMono.HideCard();
            
            var entitiesNeutral = _dataWorld.Select<CardComponent>().With<CardNeutralComponent>().GetEntities();
            var isFirstNeutralCard = false;
            foreach (var entity in entitiesNeutral)
            {
                ref var component = ref entity.GetComponent<CardComponent>();
                if (!isFirstNeutralCard)
                {
                    isFirstNeutralCard = true;
                    component.CardMono.CardOnFace(); 
                }
                else
                {
                    component.CardMono.HideCard();
                }
            }
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<DeckCardsData>();
            
            var entitiesCards = _dataWorld.Select<CardComponent>().GetEntities();

            foreach (var entity in entitiesCards)
            {
                entity.Destroy();
            }
        }
    }
}