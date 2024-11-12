using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;
using DG.Tweening;

namespace CyberNet.Core.Battle.TacticsMode.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectCardTacticsUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float time_move_card_tactics_screen = 0.35f;

        public void PreInit()
        {
            BattleTacticsUIAction.MoveCardToTacticsScreen += MoveCardToTacticsScreen;
        }
        
        private void MoveCardToTacticsScreen()
        {
            var cardToMoveTacticsScreenEntity = _dataWorld.Select<CardComponent>()
                .With<CardMoveToTacticsScreenComponent>()
                .SelectFirstEntity();
            var cardComponent = cardToMoveTacticsScreenEntity.GetComponent<CardComponent>();

            var targetPositionCard = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono.PointToTargetCard.anchoredPosition;
            
            var sequence = DOTween.Sequence();
            var scale = new Vector3(1.65f, 1.65f, 1f);//TODO забить скейл в конфиги и брать оттуда

            sequence.Append(cardComponent.CardMono.RectTransform.DOAnchorPos(targetPositionCard, time_move_card_tactics_screen))
                .Join(cardComponent.CardMono.RectTransform.DOScale(scale, time_move_card_tactics_screen))
                .OnComplete(() => EndMoveCardToTacticsScreen());
        }

        private void EndMoveCardToTacticsScreen()
        {
            var cardToMoveTacticsScreenEntity = _dataWorld.Select<CardComponent>()
                .With<CardMoveToTacticsScreenComponent>()
                .SelectFirstEntity();
            cardToMoveTacticsScreenEntity.RemoveComponent<CardMoveToTacticsScreenComponent>();
        }

        public void Destroy()
        {
            BattleTacticsUIAction.MoveCardToTacticsScreen -= MoveCardToTacticsScreen;
        }
    }
}