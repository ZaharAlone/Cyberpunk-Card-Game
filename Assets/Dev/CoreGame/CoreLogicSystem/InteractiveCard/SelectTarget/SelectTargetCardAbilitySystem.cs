using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.UI;
using DG.Tweening;
using EcsCore;
using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectTargetCardAbilitySystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectTargetCardAbilityAction.SelectTarget += SelectTarget;
        }

        public void Run()
        {
            var countEntity = _dataWorld.Select<SelectTargetCardAbilityComponent>().Count();
            if (countEntity == 0)
                return;

            var inputData = _dataWorld.OneData<InputData>();

            if (inputData.RightClick)
                CancelSelectTarget();
        }
        
        private void SelectTarget()
        {
            var isEntity = _dataWorld.Select<SelectTargetCardAbilityComponent>().TrySelectFirstEntity(out var entity);
            if (!isEntity)
                return;
            
            AbilityCancelButtonUIAction.ShowCancelButton?.Invoke();
            AnimationShowCard(entity);
        }

        private void AnimationShowCard(Entity entity)
        {
            ref var animationsCard = ref entity.GetComponent<CardComponentAnimations>();
            ref var cardComponent = ref entity.GetComponent<CardComponent>();

            var targetPosition = animationsCard.Positions;
            targetPosition.y += 75;
            animationsCard.Sequence.Kill();
            animationsCard.Sequence = DOTween.Sequence();
            animationsCard.Sequence.Append(cardComponent.RectTransform.DOLocalRotateQuaternion(animationsCard.Rotate, 0.3f))
                .Join(cardComponent.RectTransform.DOAnchorPos(targetPosition, 0.3f))
                .Join(cardComponent.RectTransform.DOScale(new Vector3(1f, 1f,1f), 0.3f));
        }
        
        private void CancelSelectTarget()
        {
            var entities = _dataWorld.Select<SelectTargetCardAbilityComponent>().GetEntities();

            foreach (var entity in entities)
            {
                entity.RemoveComponent<SelectTargetCardAbilityComponent>();
                entity.RemoveComponent<CardAbilitySelectionCompletedComponent>();
                entity.RemoveComponent<InteractiveSelectCardComponent>();
            }
            
            AbilityCancelButtonUIAction.HideCancelButton?.Invoke();
            InteractiveActionCard.ReturnAllCardInHand?.Invoke();
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
        }

        public void Destroy()
        {
            SelectTargetCardAbilityAction.SelectTarget -= SelectTarget;
        }
    }
}