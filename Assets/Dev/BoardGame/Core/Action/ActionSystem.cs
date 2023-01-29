using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class ActionSystem : IInitSystem, IPostRunEventSystem<EventUpdateBoardCard>
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            _dataWorld.CreateOneData(new ActionData());
        }

        public void PostRunEvent(EventUpdateBoardCard _) => CalculateValueCard();

        private void CalculateValueCard()
        {
            ClearTotalData();

            ref var actionData = ref _dataWorld.OneData<ActionData>();
            var entities = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardDeckComponent>().GetEntities();

            foreach (var entity in entities)
            {
                ref var component = ref entity.GetComponent<CardComponent>();

                switch (component.Stats.Ability.Type)
                {
                    case AbilityType.Attack:
                        actionData.TotalAttack += component.Ability.Value;
                        break;
                    case AbilityType.Trade:
                        actionData.TotalTrade += component.Ability.Value;
                        break;
                    case AbilityType.Influence:
                        actionData.TotalInfluence += component.Ability.Value;
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
    }
}