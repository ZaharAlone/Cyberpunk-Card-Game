using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;
using ModulesFramework.Data.Enumerators;
using DG.Tweening;
using System.Threading.Tasks;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class BoardInteractiveSystem : IRunSystem, IPostRunEventSystem<EventUpdateBoardCard>
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

        private void UpdateTableCards()
        {
            var countCard = _dataWorld.Select<CardComponent>().With<CardTableComponent>().Count();
            var entities = _dataWorld.Select<CardComponent>().With<CardTableComponent>().GetEntities();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var width = (204 + 30) * (countCard - 1);
            var start_point = width / -2;

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                cardComponent.Transform.rotation = Quaternion.identity;
                var pos = config.PlayerCardPositionInPlay;
                pos.x = start_point;

                cardComponent.CardMono.SetMovePositionAnimations(pos, config.SizeCardInTable);
                cardComponent.CardMono.CardOnFace();

                start_point += (int)(234 * config.SizeCardInTable.x);
            }
        }

        private void UpdateDiscardHub()
        {
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var ui = _dataWorld.OneData<UIData>().UIMono;

            var entitiesPlayer1 = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.Player == PlayerEnum.Player1)
                                            .With<CardMoveToDiscardComponent>()
                                            .GetEntities();
            var entitiesPlayer2 = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.Player == PlayerEnum.Player2)
                                            .With<CardMoveToDiscardComponent>()
                                            .GetEntities();

            if (viewPlayer.PlayerView == PlayerEnum.Player1)
            {
                UpdateDiscardView(entitiesPlayer1, ui.DownDiscard.localPosition, config.SizeCardInDeck);
                UpdateDiscardView(entitiesPlayer2, ui.UpDiscard.localPosition, config.SizeCardInDeck);
            }
            else
            {
                UpdateDiscardView(entitiesPlayer2, ui.DownDiscard.position, config.SizeCardInDeck);
                UpdateDiscardView(entitiesPlayer1, ui.UpDiscard.position, config.SizeCardInDeck);
            }
        }

        private void UpdateDiscardView(EntitiesEnumerable entities, Vector2 position, Vector3 size)
        {
            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                AnimationsMoveAtDiscardDeckCorotine(entity, position, size);
            }
        }

        private async void AnimationsMoveAtDiscardDeckCorotine(Entity entity, Vector3 positions, Vector3 scale)
        {
            var cardComponent = entity.GetComponent<CardComponent>();
            var sequence = DOTween.Sequence();
            sequence.Append(cardComponent.CardMono.CardConteinerTransform.DORotate(new Vector3(0, 90, 0), 0.2f));
            await sequence.AsyncWaitForCompletion();
            cardComponent.CardMono.CardOnBack();
            sequence.Append(cardComponent.CardMono.CardConteinerTransform.DORotate(new Vector3(0, 180, 0), 0.2f));
            await sequence.AsyncWaitForCompletion();
            await Task.Delay(400);

            var distance = Vector3.Distance(cardComponent.Transform.position, positions);
            var time = distance / 600;
            if (time > 0.8f)
                time = 0.8f;

            sequence.Append(cardComponent.Transform.DOMove(positions, time))
                     .Join(cardComponent.Transform.DOScale(scale, time))
                     .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 0), time / 0.5f));

            await Task.Delay((int)(1000 * time));
            entity.RemoveComponent<CardMoveToDiscardComponent>();
            entity.AddComponent(new CardDiscardComponent());
        }
    }
}