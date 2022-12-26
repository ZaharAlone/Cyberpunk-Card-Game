using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace BoardGame.Core
{
    [EcsSystem(typeof(BoardGameModule))]
    public class RoundSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            _dataWorld.CreateOneData(new RoundData { CurrentRound = 0, CurrentTurn = 1, FirstTurn = PlayerEnum.Player });
            SwitchRound();
        }

        private PlayerEnum SelectFirstTurn()
        {
            var select = Random.Range(0, 2);
            if (select == 0)
                return PlayerEnum.Player;
            else
                return PlayerEnum.Enemy;
        }

        private void SwitchRound()
        {
            _dataWorld.CreateOneFrame().AddComponent(new EventEndCurrentTurn());
        }
    }
}