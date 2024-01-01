using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class SetupCardsSystem : IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void Init()
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
                var entity = SetupCardAction.InitCard.Invoke(item, traderowParent, false);
                entity.AddComponent(new CardNeutralComponent());
                entity.AddComponent(new CardTradeRowComponent());
            }
            
            foreach (var item in deckCardsData.ShopCards)
            {
                var entity = SetupCardAction.InitCard.Invoke(item, traderowParent, false);
                entity.AddComponent(new CardTradeDeckComponent());
            }

            foreach (var playerDeckCard in deckCardsData.PlayerDeckCard)
            {
                foreach (var item in playerDeckCard.Cards)
                {
                    var entity = SetupCardAction.InitCard.Invoke(item, cardsParent, true);
                    ref var cardComponent = ref entity.GetComponent<CardComponent>();
                    cardComponent.PlayerID = playerDeckCard.IndexPlayer;
                    entity.AddComponent(new CardDrawComponent());
                    entity.AddComponent(new CardPlayerComponent());
                }
            }
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