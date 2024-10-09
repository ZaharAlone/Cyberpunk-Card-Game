using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Threading.Tasks;
using CyberNet.Core.UI;
using CyberNet.Global.Sound;
using DG.Tweening;
using ModulesFramework.Data.Enumerators;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsMoveAtDiscardDeckSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        private bool _offCore;
        
        public void PreInit()
        {
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck += UpdateDiscardHub;
            _offCore = false;
        }
        
        private void UpdateDiscardHub()
        {
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var ui = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;

            var entitiesPlayer = _dataWorld.Select<CardComponent>()
                                            .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                                            .With<CardMoveToDiscardComponent>()
                                            .GetEntities();
            
            UpdateDiscardView(entitiesPlayer, ui.CoreHudUIMono.DownDiscard, config.SizeCardInDeck);
        }

        private void UpdateDiscardView(EntitiesEnumerable entities, RectTransform targetTransform, Vector3 size)
        {
            foreach (var entity in entities)
            {
                AnimationsMoveAtDiscardDeckCoroutine(entity, targetTransform, size);
            }
        }

        private async void AnimationsMoveAtDiscardDeckCoroutine(Entity entity, RectTransform targetTransform, Vector3 scale)
        {
            if (_offCore)
                return;
            
            var cardComponent = entity.GetComponent<CardComponent>();
            var sequence = DOTween.Sequence();
            sequence.Append(cardComponent.CardMono.RectTransform.DOLocalRotate(new Vector3(0, 90, 0), 0.2f));
            await sequence.AsyncWaitForCompletion();
            
            cardComponent.CardMono.CardOnBack();
            SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.FlipCard);
            
            sequence.Append(cardComponent.CardMono.RectTransform.DOLocalRotate(new Vector3(0, 180, 0), 0.2f));
            
            await sequence.AsyncWaitForCompletion();
            if (_offCore)
                return;
            
            var targetPosition = targetTransform.position;
            var distance = Vector2.Distance(cardComponent.RectTransform.position, targetPosition);
            var time = distance / 600;
            if (time > 0.7f)
                time = 0.7f;
            
            sequence.Append(cardComponent.RectTransform.DOMove(targetPosition, time))
                     .Join(cardComponent.RectTransform.DOScale(scale, time))
                     .Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 0), time / 0.5f));

            await Task.Delay((int)(1000 * time));
            
            if (_offCore)
                return;
            
            entity.RemoveComponent<CardMoveToDiscardComponent>();
            entity.AddComponent(new CardDiscardComponent());
            
            cardComponent.RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            cardComponent.RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            cardComponent.RectTransform.position = targetPosition;
        }

        public void Destroy()
        {
            _offCore = true;
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck -= UpdateDiscardHub;
        }
    }
}