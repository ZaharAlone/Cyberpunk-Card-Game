using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Core.City;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Local
{
    [EcsSystem(typeof(CoreModule))]
    public class RoundSystem : IActivateSystem, IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            ref var selectLeader = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            _dataWorld.CreateOneData(new RoundData {
                CurrentRound = 0,
                CurrentTurn = 1,
                CurrentPlayerID = selectLeader[0].PlayerID,
                PlayerType = PlayerType.Player
            });
        }
        
        public void PreInit()
        {
            RoundAction.EndCurrentTurn += SwitchRound;
            RoundAction.StartTurn += StartTurn;
        }

        public void Init()
        {
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PositionInTurnQueue == 0)
                .SelectFirstEntity();

            var playerComponent = playerEntity.GetComponent<PlayerComponent>();
            
            _dataWorld.RiseEvent(new EventDistributionCard {
                TargetPlayerID = playerComponent.PlayerID,
                Count = rules.CountDropCard
            });
            
            UpdateUIRound(playerComponent.PlayerID);
        }
        
        private void SwitchRound()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.EndPreparationRound = false;
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var entitiesPlayer = _dataWorld.Select<PlayerComponent>().GetEntities();
            var countPlayers = _dataWorld.Select<PlayerComponent>().Count();

            var nextRoundPlayerID = 0;
            var nextRoundPlayerType = PlayerType.None;
            foreach (var entityPlayer in entitiesPlayer)
            {
                ref var componentPlayer = ref entityPlayer.GetComponent<PlayerComponent>();
                componentPlayer.PositionInTurnQueue--;

                if (componentPlayer.PositionInTurnQueue < 0)
                    componentPlayer.PositionInTurnQueue = countPlayers -1;

                if (componentPlayer.PositionInTurnQueue == 0)
                {
                    nextRoundPlayerID = componentPlayer.PlayerID;
                    nextRoundPlayerType = componentPlayer.PlayerType;
                }
            }

            roundData.CurrentPlayerID = nextRoundPlayerID;
            roundData.PlayerType = nextRoundPlayerType;
            
            _dataWorld.RiseEvent(new EventDistributionCard {
                TargetPlayerID = roundData.CurrentPlayerID,
                Count = rules.CountDropCard
            });

            if (roundData.CurrentTurn == 1)
            {
                roundData.CurrentRound++;
                roundData.CurrentTurn = 0;
            }
            else
                roundData.CurrentTurn++;
            
            UpdateUIRound(nextRoundPlayerID);
        }

        private async void UpdateUIRound(int playerID)
        {
            var uiRound = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ChangeRoundUI;
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();
            var playerViewComponent = entityPlayer.GetComponent<PlayerViewComponent>();
            
            uiRound.NewRoundView(playerViewComponent.Avatar, playerViewComponent.Name);
            await Task.Delay(2000);

            if (SelectFirstBaseAction.CheckInstallFirstBase.Invoke())
                StartTurn();
        }

        private void StartTurn()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.EndPreparationRound = true;
            
            VFXCardInteractivAction.UpdateVFXCard?.Invoke();
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
            CityAction.UpdatePresencePlayerInCity?.Invoke();
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<RoundData>();
        }
    }
}