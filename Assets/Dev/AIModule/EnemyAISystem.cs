using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace BoardGame.Core.Enemy
{
    [EcsSystem(typeof(AIModule))]
    public class EnemyAISystem : IPostRunEventSystem<EventEndCurrentTurn>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventEndCurrentTurn value)
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
            _dataWorld.RiseEvent(new EventActionAttack());
            await Task.Delay(1000);
            _dataWorld.RiseEvent(new EventActionEndTurn());
        }

        private void PlayAll()
        {
            Debug.Log("Enemy Play All Card");
            var entities = _dataWorld.Select<CardComponent>().With<CardPlayer2Component>().With<CardHandComponent>().GetEntities();

            foreach (var entity in entities)
            {
                entity.RemoveComponent<CardHandComponent>();
                entity.AddComponent(new CardDeckComponent());
            }

            _dataWorld.RiseEvent(new EventUpdateBoardCard());
        }

        private void SelectTradeCard()
        {

        }
    }
}