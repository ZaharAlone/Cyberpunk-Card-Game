using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class ActionSystem : IInitSystem, IPostRunEventSystem<EventUpdateBoardCard>, IPostRunEventSystem<EventEndCurrentTurn>
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            _dataWorld.CreateOneData(new ActionData());
        }

        public void PostRunEvent(EventUpdateBoardCard _) => CalculateValueCard();
        public void PostRunEvent(EventEndCurrentTurn _) => ClearAction();

        private void CalculateValueCard()
        {
            ClearTotalData();

            ref var actionData = ref _dataWorld.OneData<ActionData>();
            var entities = _dataWorld.Select<CardComponent>().With<CardTableComponent>().GetEntities();

            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();
                ref var selectAbility = ref entity.GetComponent<CardTableComponent>().SelectAbility;
                var abilityCard = new AbilityCard();

                if (selectAbility == SelectAbility.Ability_1)
                    abilityCard = cardComponent.Stats.Ability_0;
                else
                    abilityCard = cardComponent.Stats.Ability_1;

                switch (abilityCard.Action)
                {
                    case AbilityAction.Attack:
                        actionData.TotalAttack += abilityCard.Count;
                        break;
                    case AbilityAction.Trade:
                        actionData.TotalTrade += abilityCard.Count;
                        break;
                    case AbilityAction.Influence:
                        actionData.TotalInfluence += abilityCard.Count;
                        break;
                    case AbilityAction.DrawCard:
                        break;
                    case AbilityAction.DiscardCardEnemy:
                        break;
                    case AbilityAction.DestroyCard:
                        break;
                    case AbilityAction.DownCyberpsychosisEnemy:
                        break;
                    case AbilityAction.CloneCard:
                        break;
                    case AbilityAction.NoiseCard:
                        break;
                    case AbilityAction.ThiefCard:
                        break;
                    case AbilityAction.DestroyTradeCard:
                        break;
                    case AbilityAction.DestroyEnemyBass:
                        break;
                }
            }

            _dataWorld.RiseEvent(new EventBoardGameUpdate());
        }

        private void ClearTotalData()
        {
            ref var actionData = ref _dataWorld.OneData<ActionData>();
            actionData.TotalAttack = 0;
            actionData.TotalTrade = 0;
            actionData.TotalInfluence = 0;
        }

        private void ClearAction()
        {
            ref var actionData = ref _dataWorld.OneData<ActionData>();
            actionData.TotalAttack = 0;
            actionData.TotalTrade = 0;
            actionData.TotalInfluence = 0;
            actionData.SpendAttack = 0;
            actionData.SpendTrade = 0;
            actionData.TotalInfluence = 0;
        }
    }
}