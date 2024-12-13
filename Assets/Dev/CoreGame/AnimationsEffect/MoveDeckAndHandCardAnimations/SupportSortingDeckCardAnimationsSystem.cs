using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class SupportSortingDeckCardAnimationsSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SortingDeckCardAnimationsAction.GetTimeSortingDeck += GetTimeSortingDeck;
            SortingDeckCardAnimationsAction.GetTimeCardToHand += GetTimeCardToHand;
        }

        public float GetTimeSortingDeck(int playerTargetID)
        {
            var countWait = _dataWorld.Select<WaitEndAnimationsToStartMoveHandComponent>().Count(); ;

            if (countWait == 0)
                return 0;

            var entities = _dataWorld.Select<WaitEndAnimationsToStartMoveHandComponent>()
                                     .Where<WaitEndAnimationsToStartMoveHandComponent>(wait => wait.PlayerID == playerTargetID)
                                     .GetEntities();
            var waitTime = 0f;
            foreach (var entity in entities)
            {
                var waitComponent = entity.GetComponent<WaitEndAnimationsToStartMoveHandComponent>();
                if (waitComponent.WaitTime > waitTime)
                    waitTime = waitComponent.WaitTime;
            }

            return waitTime;
        }

        public float GetTimeCardToHand(int playerTargetID)
        {
            var countWait = _dataWorld.Select<WaitAnimationsDrawHandCardComponent>().Count(); ;

            if (countWait == 0)
                return 0;

            var entities = _dataWorld.Select<WaitAnimationsDrawHandCardComponent>()
                                     .Where<WaitAnimationsDrawHandCardComponent>(wait => wait.PlayerID == playerTargetID)
                                     .GetEntities();
            var waitTime = 0f;
            foreach (var entity in entities)
            {
                var waitComponent = entity.GetComponent<WaitAnimationsDrawHandCardComponent>();
                if (waitComponent.WaitTime > waitTime)
                    waitTime = waitComponent.WaitTime;
            }

            return waitTime;
        }

        public void Destroy()
        {
            SortingDeckCardAnimationsAction.GetTimeSortingDeck -= GetTimeSortingDeck;
            SortingDeckCardAnimationsAction.GetTimeCardToHand -= GetTimeCardToHand;
        }
    }
}