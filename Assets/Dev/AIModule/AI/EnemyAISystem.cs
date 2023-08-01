using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;
using System;
using System.Threading.Tasks;
using CyberNet.Core.UI;

namespace CyberNet.Core.Enemy
{
    [EcsSystem(typeof(VSAIModule))]
    public class EnemyAISystem : IPostRunEventSystem<EventUpdateRound>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventUpdateRound _)
        {
            var roundData = _dataWorld.OneData<RoundData>();

            if (roundData.CurrentPlayer == PlayerEnum.Player2)
                StartTurn();
        }

        private async void StartTurn()
        {
            Debug.Log("Enter Start Turn Enemy");
            await Task.Delay(1000);
            PlayAll();
            await Task.Delay(1000);
            SelectTradeCard();
            await Task.Delay(1000);
            ActionPlayerButtonEvent.ActionAttackBot?.Invoke();
            await Task.Delay(1000);
            ActionPlayerButtonEvent.ActionEndTurnBot?.Invoke();
        }

        private void PlayAll()
        {
            Debug.Log("Enemy Play All Card");
            var entities = _dataWorld.Select<CardComponent>()
                                     .Where<CardComponent>(card => card.Player == PlayerEnum.Player2)
                                     .With<CardHandComponent>().GetEntities();

            foreach (var entity in entities)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardTableComponent());
            }

            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();
        }

        private void SelectTradeCard()
        {
            
        }
    }
}