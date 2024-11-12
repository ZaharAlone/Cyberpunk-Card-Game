using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.UI;
using UnityEngine;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityAddTradePointSystem : IPreInitSystem, IDeactivateSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.AddTradePoint += CalculateAddResource;
        }
        
        private void CalculateAddResource(string guidCard)
        {
            var cardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .With<AbilityCardAddResourceComponent>()
                .SelectFirstEntity();
            
            var abilityAddResourceComponent = cardEntity.GetComponent<AbilityCardAddResourceComponent>();
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            
            actionData.TotalTrade += abilityAddResourceComponent.Count;
            
            cardEntity.RemoveComponent<AbilitySelectElementComponent>();
            cardEntity.RemoveComponent<AbilityCardAddResourceComponent>();

            if (cardEntity.HasComponent<PlayAllTradeCardComponent>())
                cardEntity.RemoveComponent<PlayAllTradeCardComponent>();

            var isSomeMoreCardPlayAll = _dataWorld.Select<PlayAllTradeCardComponent>().Count() > 0;
            
            if (isSomeMoreCardPlayAll)
                return;
            
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            
            var gameUI = _dataWorld.OneData<CoreGameUIData>();
            gameUI.BoardGameUIMono.TraderowMono.PlayEffectAddTradePoint();
            
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
        }

        public void Deactivate()
        {
            AbilityCardAction.AddTradePoint -= CalculateAddResource;
        }
    }
}