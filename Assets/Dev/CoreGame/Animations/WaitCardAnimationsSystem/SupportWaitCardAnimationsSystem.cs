using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class SupportWaitCardAnimationsSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            WaitCardAnimationsAction.GetTimeSortingDeck += GetTimeSortingDeck;
            WaitCardAnimationsAction.GetTimeCardToHand += GetTimeCardToHand;
        }

        public float GetTimeSortingDeck(PlayerEnum playerTarget)
        {
            var countWait = _dataWorld.Select<WaitCardAnimationsSortingDeckComponent>().Count(); ;

            if (countWait == 0)
                return 0;

            var entities = _dataWorld.Select<WaitCardAnimationsSortingDeckComponent>()
                                     .Where<WaitCardAnimationsSortingDeckComponent>(wait => wait.Player == playerTarget)
                                     .GetEntities();
            var waitTime = 0f;
            foreach (var entity in entities)
            {
                var waitComponent = entity.GetComponent<WaitCardAnimationsSortingDeckComponent>();
                if (waitComponent.WaitTime > waitTime)
                    waitTime = waitComponent.WaitTime;
            }

            return waitTime;
        }

        public float GetTimeCardToHand(PlayerEnum playerTarget)
        {
            var countWait = _dataWorld.Select<WaitCardAnimationsDrawHandComponent>().Count(); ;

            if (countWait == 0)
                return 0;

            var entities = _dataWorld.Select<WaitCardAnimationsDrawHandComponent>()
                                     .Where<WaitCardAnimationsDrawHandComponent>(wait => wait.Player == playerTarget)
                                     .GetEntities();
            var waitTime = 0f;
            foreach (var entity in entities)
            {
                var waitComponent = entity.GetComponent<WaitCardAnimationsDrawHandComponent>();
                if (waitComponent.WaitTime > waitTime)
                    waitTime = waitComponent.WaitTime;
            }

            return waitTime;
        }
    }
}