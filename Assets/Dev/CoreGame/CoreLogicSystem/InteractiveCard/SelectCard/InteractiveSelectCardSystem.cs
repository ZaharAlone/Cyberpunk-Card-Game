using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global.Sound;
using DG.Tweening;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Data.Enumerators;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class InteractiveSelectCardSystem : IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private const int layer_move_card = 312;
        
        public void Init()
        {
            InteractiveActionCard.SelectCardMap += SelectCardMap;
            BattleTacticsUIAction.SelectCardTactics += SelectCardTactics;
            InteractiveActionCard.DeselectCardMap += DeselectCardMap;
            BattleTacticsUIAction.DeselectCardTactics += DeselectCardTactics;
            InteractiveActionCard.ReturnAllCardInHand += ReturnAllCard;
        }

        private void SelectCardMap(string guidCard)
        {
            if (_dataWorld.Select<InteractiveSelectCardComponent>().Count() != 0)
                return;
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            if (roundData.PauseInteractive)
                return;

            var isFindTargetCard = _dataWorld.Select<CardComponent>()
                        .Where<CardComponent>(card => card.GUID == guidCard)
                        .Without<CardAbilitySelectionCompletedComponent>()
                        .Without<CardDrawComponent>()
                        .Without<CardDistributionComponent>()
                        .TrySelectFirstEntity(out var entityCard);

            if (!isFindTargetCard)
                return;

            if (entityCard.HasComponent<CardSelectInTacticsScreenComponent>())
                return;
            
            var cardComponent = entityCard.GetComponent<CardComponent>();
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            if (currentPlayerID != cardComponent.PlayerID && !entityCard.HasComponent<CardTradeRowComponent>())
                return;

            AnimationsSelectCard(entityCard);
        }

        private void SelectCardTactics(string guidCard)
        {
            if (_dataWorld.Select<InteractiveSelectCardComponent>().Count() != 0)
                return;
            
            var isFindTargetCard = _dataWorld.Select<CardComponent>()
                .With<CardTacticsComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .TrySelectFirstEntity(out var entityCard);
            
            if (!isFindTargetCard)
                return;
            
            AnimationsSelectCard(entityCard);
        }

        private void AnimationsSelectCard(Entity entityCard)
        {
            ClearSelectComponent();
            entityCard.AddComponent(new InteractiveSelectCardComponent());
            
            var animComponent = CreateAnimationsComponent(entityCard);
            
            SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.SelectCard);

            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            ref var cardComponent = ref entityCard.GetComponent<CardComponent>();

            var gameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var scaleCard = gameConfig.SizeSelectCardHand;
            if (entityCard.HasComponent<CardTradeRowComponent>())
                scaleCard = gameConfig.SizeSelectCardTradeRow;

            animComponent.Sequence = DOTween.Sequence();
            animComponent.Sequence.Append(cardComponent.RectTransform.DOLocalRotateQuaternion(Quaternion.identity, 0.23f))
                                  .Join(cardComponent.RectTransform.DOScale(scaleCard, 0.23f));

            cardComponent.Canvas.sortingOrder = layer_move_card;
            
            if (cardComponent.PlayerID == currentPlayerID)
            {
                var pos = animComponent.Positions;
                pos.y = -340;
                animComponent.Sequence.Join(cardComponent.RectTransform.DOAnchorPos(pos, 0.23f));
                entityCard.AddComponent(animComponent);
                var index = entityCard.GetComponent<CardSortingIndexComponent>().Index;

                if (entityCard.HasComponent<CardHandComponent>())
                    MoveOtherCardsInHand(index);
                else
                    MoveOtherCardsInTactics(index);
                
                CoreElementInfoPopupAction.OpenPopupCard?.Invoke(cardComponent.GUID, false);
            }
            else
            {
                var pos = animComponent.Positions;
                pos.y = -250;
                animComponent.Sequence.Join(cardComponent.RectTransform.DOAnchorPos(pos, 0.23f));
                entityCard.AddComponent(animComponent);
                
                CoreElementInfoPopupAction.OpenPopupCard?.Invoke(cardComponent.GUID, true);
            }
        }

        private void ClearSelectComponent()
        {
            var entities = _dataWorld.Select<CardComponent>()
                            .With<InteractiveSelectCardComponent>()
                            .GetEntities();

            foreach (var entity in entities)
                entity.RemoveComponent<InteractiveSelectCardComponent>();
        }

        private CardComponentAnimations CreateAnimationsComponent(Entity entityCard)
        {
            var cardComponent = entityCard.GetComponent<CardComponent>();
            if (entityCard.HasComponent<CardComponentAnimations>())
            {
                var animComponent = entityCard.GetComponent<CardComponentAnimations>();
                animComponent.Sequence.Kill();
                return animComponent;
            }
            else
            {
                var animComponent = new CardComponentAnimations();
                
                animComponent.Positions = cardComponent.RectTransform.anchoredPosition;
                animComponent.Rotate = cardComponent.RectTransform.localRotation;
                animComponent.Scale = cardComponent.RectTransform.localScale;
                animComponent.SortingOrder = cardComponent.Canvas.sortingOrder;

                entityCard.AddComponent(animComponent);
                return animComponent;
            }
        }

        private void MoveOtherCardsInHand(int indexTargetCard)
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var entities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardHandComponent>()
                .With<CardSortingIndexComponent>()
                .Without<InteractiveSelectCardComponent>()
                .GetEntities();

            MoveOtherCards(indexTargetCard, entities);
        }

        private void MoveOtherCardsInTactics(int indexTargetCard)
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var entities = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardTacticsComponent>()
                .With<CardSortingIndexComponent>()
                .GetEntities();

            MoveOtherCards(indexTargetCard, entities);
        }
        
        private void MoveOtherCards(int targetIndex, EntitiesEnumerable entitiesCards)
        {
            foreach (var entity in entitiesCards)
            {
                ref var index = ref entity.GetComponent<CardSortingIndexComponent>().Index;
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                var cardAnimations = new CardComponentAnimations();

                if (entity.HasComponent<CardComponentAnimations>())
                {
                    cardAnimations = entity.GetComponent<CardComponentAnimations>();
                    cardAnimations.Sequence.Kill();
                }
                else
                {
                    cardAnimations.Positions = cardComponent.RectTransform.anchoredPosition;
                    cardAnimations.Rotate = cardComponent.RectTransform.localRotation;
                    cardAnimations.Scale = cardComponent.RectTransform.localScale;
                    cardAnimations.SortingOrder = cardComponent.Canvas.sortingOrder;
                }
                cardAnimations.Sequence = DOTween.Sequence();

                var targetPos = cardAnimations.Positions;
                if (index < targetIndex)
                {
                    if (index == targetIndex - 1)
                        targetPos.x -= 50;
                    else
                        targetPos.x -= 25;
                }
                else if (index > targetIndex)
                {
                    if (index == targetIndex + 1)
                        targetPos.x += 50;
                    else
                        targetPos.x += 25;
                }

                cardAnimations.Sequence.Append(cardComponent.RectTransform.DOAnchorPos(targetPos, 0.15f))
                                       .Join(cardComponent.RectTransform.DOLocalRotateQuaternion(cardAnimations.Rotate, 0.3f))
                                       .Join(cardComponent.RectTransform.DOScale(cardAnimations.Scale, 0.3f));

                cardComponent.Canvas.sortingOrder = cardAnimations.SortingOrder;
                entity.AddComponent(cardAnimations);
            }
        }

        private void DeselectCardMap(string guid)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            if (roundData.CurrentGameStateMapVSArena == GameStateMapVSArena.Map && _dataWorld.Select<SelectTargetCardAbilityComponent>().Count() > 0)
                return;
            
            if (roundData.PauseInteractive)
                return;
            
            var isEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .With<InteractiveSelectCardComponent>()
                .With<CardComponentAnimations>()
                .Without<CardDistributionComponent>()
                .Without<CardDrawComponent>()
                .TrySelectFirstEntity(out var entity);
            
            if (isEntity)
                DeselectCard(entity);
        }

        private void DeselectCardTactics(string guid)
        {
            var isEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guid)
                .With<CardTacticsComponent>()
                .With<InteractiveSelectCardComponent>()
                .With<CardComponentAnimations>()
                .TrySelectFirstEntity(out var entity);
            
            if (isEntity)
                DeselectCard(entity);
        }
        
        private void DeselectCard(Entity entityCard)
        {
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            if (entityCard.HasComponent<CardHandComponent>())
                ReturnAllCard();
            else
                ReturnCardAnimations(entityCard);
        }

        private void ReturnAllCard()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var entities = _dataWorld.Select<CardComponent>()
                                        .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                                        .With<CardHandComponent>()
                                        .With<CardComponentAnimations>()
                                        .Without<InteractiveSelectCardComponent>()
                                        .GetEntities();

            foreach (var entity in entities)
                ReturnCardAnimations(entity);
        }

        private void ReturnCardAnimations(Entity entity)
        {
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var animationsCard = ref entity.GetComponent<CardComponentAnimations>();
            
            animationsCard.Sequence.Kill();
            cardComponent.Canvas.sortingOrder = animationsCard.SortingOrder;
            animationsCard.Sequence = DOTween.Sequence();
            animationsCard.Sequence.Append(cardComponent.RectTransform.DOLocalRotateQuaternion(animationsCard.Rotate, 0.3f))
                                .Join(cardComponent.RectTransform.DOAnchorPos(animationsCard.Positions, 0.3f))
                                .Join(cardComponent.RectTransform.DOScale(animationsCard.Scale, 0.3f))
                                .OnComplete(() => FinishDeselect(entity));
        }

        private void FinishDeselect(Entity entity)
        {
            entity.RemoveComponent<CardComponentAnimations>();
        }

        public void Destroy()
        {
            InteractiveActionCard.SelectCardMap -= SelectCardMap;
            BattleTacticsUIAction.SelectCardTactics -= SelectCardTactics;
            InteractiveActionCard.DeselectCardMap -= DeselectCardMap;
            BattleTacticsUIAction.DeselectCardTactics -= DeselectCardTactics;
            InteractiveActionCard.ReturnAllCardInHand -= ReturnAllCard;
        }
    }
}