using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using CyberNet.Core.UI;
using CyberNet.Tools;
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
            var viewPlayerID = _dataWorld.OneData<CurrentPlayerViewScreenData>().CurrentPlayerID;
            var ui = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;

            var entitiesPlayer1 = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.PlayerID == viewPlayerID)
                                            .With<CardMoveToDiscardComponent>()
                                            .GetEntities();
            var entitiesPlayer2 = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.PlayerID == viewPlayerID)
                                            .With<CardMoveToDiscardComponent>()
                                            .GetEntities();
            
            //TODO: старый код
            /*
            if (viewPlayerID == PlayerEnum.Player1)
            {
                UpdateDiscardView(entitiesPlayer1, ui.CoreHudUIMono.DownDiscard, config.SizeCardInDeck);
                //UpdateDiscardView(entitiesPlayer2, ui.CoreHudUIMono.UpDiscard.localPosition, config.SizeCardInDeck);
            }
            else
            {
                UpdateDiscardView(entitiesPlayer2, ui.CoreHudUIMono.DownDiscard, config.SizeCardInDeck);
                //UpdateDiscardView(entitiesPlayer1, ui.CoreHudUIMono.UpDiscard.position, config.SizeCardInDeck);
            }*/
        }

        private void UpdateDiscardView(EntitiesEnumerable entities, RectTransform targetTransform, Vector3 size)
        {
            foreach (var entity in entities)
            {
                AnimationsMoveAtDiscardDeckCorotine(entity, targetTransform, size);
            }
        }

        private async void AnimationsMoveAtDiscardDeckCorotine(Entity entity, RectTransform targetTransform, Vector3 scale)
        {
            var cardComponent = entity.GetComponent<CardComponent>();
            var sequence = DOTween.Sequence();
            sequence.Append(cardComponent.CardMono.RectTransform.DOLocalRotate(new Vector3(0, 90, 0), 0.2f));
            await sequence.AsyncWaitForCompletion();
            cardComponent.CardMono.CardOnBack();
            sequence.Append(cardComponent.CardMono.RectTransform.DOLocalRotate(new Vector3(0, 180, 0), 0.2f));
            await sequence.AsyncWaitForCompletion();
            await Task.Delay(400);

            var targetPosition = targetTransform.position;
            var distance = Vector2.Distance(cardComponent.RectTransform.position, targetPosition);
            var time = distance / 100;
            if (time > 0.8f)
                time = 0.8f;
            
            sequence.Append(cardComponent.RectTransform.DOMove(targetPosition, time))
                     .Join(cardComponent.RectTransform.DOScale(scale, time))
                     .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 0), time / 0.5f));

            await Task.Delay((int)(1000 * time));
            entity.RemoveComponent<CardMoveToDiscardComponent>();
            entity.AddComponent(new CardDiscardComponent());
        }
    }
}