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
    [EcsSystem(typeof(CoreModule))]
    public class BoardInteractiveSystem : IRunSystem, IPostRunEventSystem<EventUpdateBoardCard>, IPostRunEventSystem<EventUpdateDeckCard>
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
                var ui = _dataWorld.OneData<UIData>();

                if (distance > 150)
                    ui.UIMono.InteractiveZoneImage.color = new Color(255, 255, 255, 255);
                else
                    ui.UIMono.InteractiveZoneImage.color = new Color(255, 255, 255, 0);
            }
        }

        public void PostRunEvent(EventUpdateBoardCard _)
        {
            UpdateTableCards();
            UpdateDiscardHub();
        }

        public void PostRunEvent(EventUpdateDeckCard _) => UpdateDeck();

        private void UpdateTableCards()
        {
            var countCard = _dataWorld.Select<CardComponent>().With<CardDeckComponent>().Count();
            var entities = _dataWorld.Select<CardComponent>().With<CardDeckComponent>().GetEntities();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var width = (204 + 30) * (countCard - 1);
            var start_point = width / -2;

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.Transform.rotation = Quaternion.identity;
                var pos = config.PlayerCardPositionInPlay;
                pos.x = start_point;

                cardComponent.CardMono.SetMovePositionAnimations(pos, config.NormalSize);
                cardComponent.CardMono.CardOnFace();

                start_point += (204 + 30);
            }
        }

        private void UpdateDiscardHub()
        {
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var ui = _dataWorld.OneData<UIData>().UIMono;

            var entitiesPlayer1 = _dataWorld.Select<CardComponent>().With<CardPlayer1Component>().With<CardDiscardComponent>().GetEntities();
            var entitiesPlayer2 = _dataWorld.Select<CardComponent>().With<CardPlayer2Component>().With<CardDiscardComponent>().GetEntities();

            if (viewPlayer.PlayerView == PlayerEnum.Player1)
            {
                UpdateDiscardView(entitiesPlayer1, ui.DiscardDownCard.localPosition, config.SizeCardPlayerDown, false);
                UpdateDiscardView(entitiesPlayer2, ui.DiscardUpCard.localPosition, config.SizeCardPlayerUp, true);
            }
            else
            {
                UpdateDiscardView(entitiesPlayer2, ui.DiscardDownCard.position, config.SizeCardPlayerDown, false);
                UpdateDiscardView(entitiesPlayer1, ui.DiscardUpCard.position, config.SizeCardPlayerUp, true);
            }
        }

        private void UpdateDiscardView(EntitiesEnumerable entities, Vector2 position, Vector3 size, bool isEnemy)
        {
            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                ref var discardCard = ref entity.GetComponent<CardDiscardComponent>();
                cardComponent.CardMono.AnimationsMoveAtDiscardDeck(position, size);
            }
        }

        private void UpdateDeck()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            if (roundData.CurrentPlayer == PlayerEnum.Player1)
            {
                var stackCard = _dataWorld.Select<CardComponent>()
                                          .With<CardPlayer1Component>()
                                          .Without<CardDiscardComponent>()
                                          .Without<CardHandComponent>()
                                          .GetEntities();

                foreach (var entity in stackCard)
                {
                    ref var component = ref entity.GetComponent<CardComponent>();
                    component.Transform.position = config.PositionsCardDeckPlayer;
                }
            }
            else
            {
                var stackCard = _dataWorld.Select<CardComponent>()
                                          .With<CardPlayer2Component>()
                                          .Without<CardDiscardComponent>()
                                          .Without<CardHandComponent>()
                                          .GetEntities();

                foreach (var entity in stackCard)
                {
                    ref var component = ref entity.GetComponent<CardComponent>();
                    component.Transform.position = config.PositionsCardDeckEnemy;
                }
            }
        }
    }
}