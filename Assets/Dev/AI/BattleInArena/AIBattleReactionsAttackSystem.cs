using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.AI;
using CyberNet.Core.Arena;
using CyberNet.Core.EnemyTurnView;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class AIBattleReactionsAttackSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.CheckBlockAttack += CheckBlockAttack;
            ArenaAction.CheckReactionsShooting += SelectAndDiscardCardToBlockAttack;
        }
        
        private bool CheckBlockAttack()
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();

            var countCardInHandPlayer = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == targetUnitComponent.PlayerControlID)
                .With<CardHandComponent>()
                .Count();

            return countCardInHandPlayer > 0;
        }
        
        private bool SelectAndDiscardCardToBlockAttack()
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();

            var cardsEntities = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.PlayerID == targetUnitComponent.PlayerControlID)
                .GetEntities();

            var minValueCard = 99;
            var selectCardGUID = "";
                    
            foreach (var cardEntity in cardsEntities)
            {
                var cardComponent = cardEntity.GetComponent<CardComponent>();
                var valueAbility_0 = CalculateValueCardAction.CalculateValueCardAbility.Invoke(cardComponent.Ability_0);
                var valueAbility_1 = CalculateValueCardAction.CalculateValueCardAbility.Invoke(cardComponent.Ability_1);

                var maxValueCard = Mathf.Max(valueAbility_0, valueAbility_1);

                if (maxValueCard < minValueCard)
                {
                    minValueCard = maxValueCard;
                    selectCardGUID = cardComponent.GUID;
                }
            }
                    
            if (selectCardGUID == "")
                return false;

            var cardToDiscardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == selectCardGUID)
                .SelectFirstEntity();

            var cardToDiscardComponent = cardToDiscardEntity.GetComponent<CardComponent>();
                    
            cardToDiscardEntity.RemoveComponent<CardHandComponent>();
            cardToDiscardEntity.AddComponent(new CardStartMoveToTableComponent());
            
            EnemyTurnViewUIAction.ShowViewEnemyCardSetPlayer?
                .Invoke(EnemyTurnActionType.DiscardCard, cardToDiscardComponent.Key, targetUnitComponent.PlayerControlID);
            
            return true;
        }

        public void Destroy()
        {
            ArenaAction.CheckBlockAttack -= CheckBlockAttack;
            ArenaAction.CheckReactionsShooting -= SelectAndDiscardCardToBlockAttack;
        }
    }
}