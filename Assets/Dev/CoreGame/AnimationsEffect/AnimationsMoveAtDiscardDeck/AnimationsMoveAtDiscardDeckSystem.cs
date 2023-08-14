using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using CyberNet.Core.UI;
using DG.Tweening;
using ModulesFramework.Data.Enumerators;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsMoveAtDiscardDeckSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck += UpdateDiscardHub;
        }
        
        private void UpdateDiscardHub()
        {
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var ui = _dataWorld.OneData<CoreUIData>().BoardGameUIMono;

            var entitiesPlayer1 = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.Player == PlayerEnum.Player1)
                                            .With<CardMoveToDiscardComponent>()
                                            .GetEntities();
            var entitiesPlayer2 = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.Player == PlayerEnum.Player2)
                                            .With<CardMoveToDiscardComponent>()
                                            .GetEntities();
            //TODO: старый код
            if (viewPlayer.PlayerView == PlayerEnum.Player1)
            {
                UpdateDiscardView(entitiesPlayer1, ui.CoreHudUIMono.DownDiscard.localPosition, config.SizeCardInDeck);
                //UpdateDiscardView(entitiesPlayer2, ui.CoreHudUIMono.UpDiscard.localPosition, config.SizeCardInDeck);
            }
            else
            {
                UpdateDiscardView(entitiesPlayer2, ui.CoreHudUIMono.DownDiscard.position, config.SizeCardInDeck);
                //UpdateDiscardView(entitiesPlayer1, ui.CoreHudUIMono.UpDiscard.position, config.SizeCardInDeck);
            }
        }

        private void UpdateDiscardView(EntitiesEnumerable entities, Vector2 position, Vector3 size)
        {
            foreach (var entity in entities)
            {
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