using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;
using DG.Tweening;

namespace CyberNet.Core.Battle.TacticsMode
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectCardTacticsUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.MoveCardToTacticsScreen += MoveCardToTacticsScreen;
        }
        
        private void MoveCardToTacticsScreen()
        {
            var cardToMoveTacticsScreenEntity = _dataWorld.Select<CardComponent>().With<CardMoveToTacticsScreenComponent>().SelectFirstEntity();
            var cardComponent = cardToMoveTacticsScreenEntity.GetComponent<CardComponent>();

            var uiTacticsTargetMoveCard = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono.PointToTargetCard;
            
            var sequence = DOTween.Sequence();
            var distanceToTarget = Vector3.Distance(cardComponent.RectTransform.position, uiTacticsTargetMoveCard.anchoredPosition);
            
            var scale = new Vector3(1.65f, 1.65f, 1f);
            
            var time = distanceToTarget / 800;
            sequence.Append(cardComponent.CardMono.RectTransform.DOAnchorPos(uiTacticsTargetMoveCard.anchoredPosition, time))
                .Join(cardComponent.CardMono.RectTransform.DOScale(scale, time))
                .OnComplete(() => EndMoveCardToTacticsScreen());
        }

        private void EndMoveCardToTacticsScreen()
        {
            var cardToMoveTacticsScreenEntity = _dataWorld.Select<CardComponent>().With<CardMoveToTacticsScreenComponent>().SelectFirstEntity();
            cardToMoveTacticsScreenEntity.RemoveComponent<CardMoveToTacticsScreenComponent>();
        }

        public void Destroy()
        {
            BattleTacticsUIAction.MoveCardToTacticsScreen -= MoveCardToTacticsScreen;
        }
    }
}