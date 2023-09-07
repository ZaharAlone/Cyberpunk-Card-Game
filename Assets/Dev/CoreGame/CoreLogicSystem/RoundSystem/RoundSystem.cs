using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using System;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Core.UI;

namespace CyberNet.Local
{
    [EcsSystem(typeof(CoreModule))]
    public class RoundSystem : IActivateSystem, IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;

            //TODO переписать
//            _dataWorld.RiseEvent(new EventDistributionCard { TargetPlayerID = PlayerEnum.Player1, Count = rules.CountDropCard });
//            _dataWorld.RiseEvent(new EventDistributionCard { TargetPlayerID = PlayerEnum.Player2, Count = rules.CountDropCard });
        }

        public void PreInit()
        {
            RoundAction.EndCurrentTurn += SwitchRound;
        }

        private void SwitchRound()
        {
            /*
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.RiseEvent(new EventDistributionCard { TargetPlayerID = roundData.CurrentPlayer, Count = rules.CountDropCard });

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

            RoundAction.UpdateTurn?.Invoke();
            UpdateUIRound(roundData.CurrentPlayer);*/
        }

        private async void UpdateUIRound(PlayerEnum playersRound)
        {
            /*
            var viewPlayer = _dataWorld.OneData<CurrentPlayerViewScreenData>();
            var ui = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            
            if (playersRound == viewPlayer.CurrentPlayerView)
                ui.ChangeRoundUI.PlayerRound();
            else
                ui.ChangeRoundUI.EnemyRound();

            await Task.Delay(2000);
            VFXCardInteractivAction.UpdateVFXCard?.Invoke();*/
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<RoundData>();
        }
    }
}