using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using System;
using BoardGame.Core;

namespace BoardGame.Local
{
    [EcsSystem(typeof(LocalGameModule))]
    public class RoundSystem : IActivateSystem, IPostRunEventSystem<EventEndCurrentTurn>
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            var firstPlayer = SelectFirstPlayer.Select();
            _dataWorld.CreateOneData(new RoundData { CurrentRound = 0, CurrentTurn = 1, CurrentPlayer = firstPlayer });

            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.RiseEvent(new EventDistributionCard { Target = firstPlayer, Count = rules.CountDropCard });
        }

        public void PostRunEvent(EventEndCurrentTurn _) => SwitchRound();

        private void SwitchRound()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            if (roundData.CurrentTurn == 1)
            {
                roundData.CurrentRound++;
                roundData.CurrentTurn = 0;
            }
            else
                roundData.CurrentTurn++;

            if (roundData.CurrentPlayer == PlayerEnum.Player1)
                roundData.CurrentPlayer = PlayerEnum.Player2;
            else
                roundData.CurrentPlayer = PlayerEnum.Player1;

            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.RiseEvent(new EventDistributionCard { Target = roundData.CurrentPlayer, Count = rules.CountDropCard });
        }
    }
}