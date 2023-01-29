using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;
using ModulesFramework.Data.Enumerators;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class BoardInteractiveSystem : IRunSystem, IPostRunEventSystem<EventUpdateBoardCard>, IPostRunEventSystem<EventUpdateStackCard>
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var entities = _dataWorld.Select<CardComponent>().With<InteractiveMoveComponent>().GetEntities();

            foreach (var entity in entities)
            {
                ref var componentMove = ref entity.GetComponent<InteractiveMoveComponent>();
                ref var componentCard = ref entity.GetComponent<CardComponent>();

                var distance = componentCard.Transform.position.y - componentMove.StartCardPosition.y;
                var ui = _dataWorld.OneData<BoardGameUIComponent>();

                if (distance > 150)
                    ui.UIMono.InteractiveZoneImage.color = new Color(255, 255, 255, 255);
                else
                    ui.UIMono.InteractiveZoneImage.color = new Color(255, 255, 255, 0);
            }
        }

        public void PostRunEvent(EventUpdateBoardCard _)
        {
            UpdateDeck();
            UpdateDiscardHub();
        }

        public void PostRunEvent(EventUpdateStackCard _) => UpdateStack();

        private void UpdateDeck()
        {
            var countCard = _dataWorld.Select<CardComponent>().With<CardDeckComponent>().Count();
            var entities = _dataWorld.Select<CardComponent>().With<CardDeckComponent>().GetEntities();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var width = (204 + 30) * (countCard - 1);
            var start_point = width / -2;

            foreach (var entity in entities)
            {
                ref var card = ref entity.GetComponent<CardComponent>();
                card.Transform.rotation = Quaternion.identity;
                card.Transform.localScale = config.NormalSize;

                var pos = config.PlayerCardPositionInPlay;
                pos.x = start_point;
                card.Transform.position = pos;
                card.CardMono.CardOnFace();

                start_point += (204 + 30);
            }
        }

        private void UpdateDiscardHub()
        {
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var round = _dataWorld.OneData<RoundData>();
            
            if (round.CurrentPlayer == PlayerEnum.Player)
            {
                var entities = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardDiscardComponent>().GetEntities();
                UpdateDiscardView(entities, config.PlayerCardDiscardPosition, config.SizeCardInDeckAndDrop, false);
            }
            else
            {
                var entities = _dataWorld.Select<CardComponent>().With<CardEnemyComponent>().With<CardDiscardComponent>().GetEntities();
                UpdateDiscardView(entities, config.EnemyCardDiscardPosition, config.SizeCardEnemy, true);
            }
        }

        private void UpdateDiscardView(EntitiesEnumerable entities, Vector2 position, Vector3 size, bool isEnemy)
        {
            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                ref var discardCard = ref entity.GetComponent<CardDiscardComponent>();
                cardComponent.GO.transform.position = position;
                cardComponent.GO.transform.localScale = size;

                if (!isEnemy)
                {
                    if (discardCard.IsLast)
                        cardComponent.CardMono.Canvas.sortingOrder = 3;
                    else
                        cardComponent.CardMono.Canvas.sortingOrder = 2;
                }
                else
                    cardComponent.CardMono.CardOnBack();
            }
        }

        private void UpdateStack()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            if (roundData.CurrentPlayer == PlayerEnum.Player)
            {
                var stackCard = _dataWorld.Select<CardComponent>()
                                          .With<CardPlayerComponent>()
                                          .Without<CardDiscardComponent>()
                                          .Without<CardHandComponent>()
                                          .GetEntities();

                foreach (var entity in stackCard)
                {
                    ref var component = ref entity.GetComponent<CardComponent>();
                    component.Transform.position = config.PositionsCardDeckPlayer;
                    component.CardMono.CardOnBack();
                }
            }
            else
            {
                var stackCard = _dataWorld.Select<CardComponent>()
                                          .With<CardEnemyComponent>()
                                          .Without<CardDiscardComponent>()
                                          .Without<CardHandComponent>()
                                          .GetEntities();

                foreach (var entity in stackCard)
                {
                    ref var component = ref entity.GetComponent<CardComponent>();
                    component.Transform.position = config.PositionsCardDeckEnemy;
                    component.CardMono.CardOnBack();
                }
            }
        }
    }
}