using CyberNet.Core.AI;
using CyberNet.Core.City;
using CyberNet.Core.UI;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityCardMoveUnitSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;
        private bool _isSubscription;
        public void PreInit()
        {
            AbilityCardAction.MoveUnit += MoveUnit;
        }
        
        private void MoveUnit()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                AbilityAIAction.MoveUnit?.Invoke();
                return;
            }

            _dataWorld.NewEntity().AddComponent(new AbilityCardMoveUnitComponent());

            roundData.PauseInteractive = true;
            AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.SquadMove, 0, false);
            CityAction.ShowWherePlayerCanMove?.Invoke();
            CityAction.SelectTower += SelectTower;
        }
        
        private void SelectTower(string towerGUID)
        {
            ref var uiActionSelectCard = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilitySelectElementUIMono;
            //AbilitySelectElementAction.SelectElement?.Invoke();

            var entityMoveCard = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity();
            ref var moveCardComponent = ref entityMoveCard.GetComponent<AbilityCardMoveUnitComponent>();
            moveCardComponent.SelectTowerGUID = towerGUID;
            
            if (!_isSubscription)
            {
                _isSubscription = true;
                AbilityCardAction.ConfimSelectElement += ConfimSelectTower;   
            }
        }

        private void ConfimSelectTower()
        {
            _isSubscription = false;
            CityAction.SelectTower -= SelectTower;
            AbilityCardAction.ConfimSelectElement -= ConfimSelectTower;
            
            var cardEntity = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .Where<AbilitySelectElementComponent>(selectCard => selectCard.AbilityCard.AbilityType == AbilityType.SquadMove)
                .SelectFirstEntity();
            
            cardEntity.RemoveComponent<AbilitySelectElementComponent>();
            CityAction.UpdateCanInteractiveMap?.Invoke();

            SelectUnitToMove();
        }

        private void SelectUnitToMove()
        {
            var canMoveUnitComponent = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity()
                .GetComponent<AbilityCardMoveUnitComponent>();
            
            CityAction.ShowWherePlayerCanMoveFrom?.Invoke(canMoveUnitComponent.SelectTowerGUID);
            AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.SquadMove, 1, false);
            CityAction.SelectUnit += SelectUnit;
        }

        private void SelectUnit(string GUID, int index)
        {
            
        }

        private void DeselectUnit()
        {
            
        }

        private void ConfimMove()
        {
            
        }

        private void AnimationMoveToTarget()
        {
            
        }

        private void FinishAnimation()
        {
            
        }
    }
}