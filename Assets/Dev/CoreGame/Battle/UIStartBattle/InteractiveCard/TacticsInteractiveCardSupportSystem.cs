using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.UI;

namespace CyberNet.Core.Battle.TacticsMode.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class TacticsInteractiveCardSupportSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.CheckIsSelectCardTactics += CheckIsSelectCardTactics;
        }
        
        private void CheckIsSelectCardTactics(string guidNextSelectCard)
        {
            var isSelectCardTactics = _dataWorld.Select<CardComponent>()
                .With<CardSelectInTacticsScreenComponent>()
                .TrySelectFirstEntity(out var selectTacticsCardEntity);
            
            if (!isSelectCardTactics)
                return;

            var cardComponent = selectTacticsCardEntity.GetComponent<CardComponent>();
            
            if (cardComponent.GUID == guidNextSelectCard)
                return;

            selectTacticsCardEntity.RemoveComponent<CardSelectInTacticsScreenComponent>();
            selectTacticsCardEntity.AddComponent(new CardHandComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInTacticsScreen?.Invoke();
        }

        public void Destroy()
        {
            BattleTacticsUIAction.CheckIsSelectCardTactics -= CheckIsSelectCardTactics;
        }
    }
}