using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Battle.TacticsMode.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class CreateCardInTacticsScreenSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.CreateCardTactics += CreateCardTactics;
        }
        
        private void CreateCardTactics()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var cardInPlayerHandEntities = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .GetEntities();

            var battleTacticsUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            var cardInTacticsScreen = _dataWorld.OneData<BattleTacticsData>().CardFoTacticsScreen;

            foreach (var entityCard in cardInPlayerHandEntities)
            {
                var cardComponent = entityCard.GetComponent<CardComponent>();
                
                var nextCard = Object.Instantiate(cardInTacticsScreen, battleTacticsUI.CardsContainer);
                nextCard.SetGUID(cardComponent.GUID);
                SetupCardAction.SetViewCardNotInitToDeck?.Invoke(nextCard.CardMono, cardComponent.Key);
                
                var cardTacticsComponent = new CardTacticsComponent
                {
                    InteractiveCardMono = nextCard,
                };
                
                var cardSortingComponent = entityCard.GetComponent<CardSortingIndexComponent>();
                var copyCardComponent = cardComponent;
                copyCardComponent.CardMono = nextCard.CardMono;
                copyCardComponent.RectTransform = nextCard.CardMono.RectTransform;
                copyCardComponent.Canvas = nextCard.CardMono.Canvas;

                var nextCardEntity = _dataWorld.NewEntity();
                nextCardEntity.AddComponent(cardTacticsComponent);
                nextCardEntity.AddComponent(copyCardComponent);
                nextCardEntity.AddComponent(cardSortingComponent);
                nextCardEntity.AddComponent(new CardHandComponent());
            }
            
            CardAnimationsHandAction.AnimationsFanCardInTacticsScreen?.Invoke();
        }

        public void Destroy()
        {
            BattleTacticsUIAction.CreateCardTactics -= CreateCardTactics;
        }
    }
}