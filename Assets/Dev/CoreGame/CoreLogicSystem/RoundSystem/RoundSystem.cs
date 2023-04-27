using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using System;
using BoardGame.Core;
using BoardGame.Core.UI;

namespace BoardGame.Local
{
    [EcsSystem(typeof(CoreModule))]
    public class RoundSystem : IActivateSystem, IPostRunEventSystem<EventEndCurrentTurn>
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            _dataWorld.RiseEvent(new EventDistributionCard { Target = PlayerEnum.Player1, Count = rules.CountDropCard });
            _dataWorld.RiseEvent(new EventDistributionCard { Target = PlayerEnum.Player2, Count = rules.CountDropCard });
        }

        public void PostRunEvent(EventEndCurrentTurn _) => SwitchRound();

        private void SwitchRound()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.RiseEvent(new EventDistributionCard { Target = roundData.CurrentPlayer, Count = rules.CountDropCard });

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

            _dataWorld.RiseEvent(new EventUpdateRound());
            UpdateUIRound();
        }

        private void UpdateUIRound()
        {
            ref var ui = ref _dataWorld.OneData<UIData>().UIMono;
            ui.ChangeRoundUI.OnNewRound();
        }
    }
}