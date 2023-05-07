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
                    case AbilityAction.attack:
                        actionData.TotalAttack += abilityCard.Count;
                        break;
                    case AbilityAction.trade:
                        actionData.TotalTrade += abilityCard.Count;
                        break;
                    case AbilityAction.influence:
                        actionData.TotalInfluence += abilityCard.Count;
                        break;
                    case AbilityAction.drawCard:
                        break;
                    case AbilityAction.discardCard:
                        break;
                    case AbilityAction.destroyCard:
                        break;
                    case AbilityAction.upСyberpsychosis:
                        break;
                    case AbilityAction.downCyberpsychosis:
                        break;
                    case AbilityAction.cloneCard:
                        break;
                    case AbilityAction.noiseCard:
                        break;
                    case AbilityAction.thiefCard:
                        break;
                    default:
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