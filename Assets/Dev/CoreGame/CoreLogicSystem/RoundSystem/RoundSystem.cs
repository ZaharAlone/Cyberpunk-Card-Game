using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Tutorial;
using UnityEngine;

namespace CyberNet.Local
{
    [EcsSystem(typeof(CoreModule))]
    public class RoundSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            RoundAction.EndCurrentTurn += SwitchRound;
            RoundAction.StartTurn += StartTurn;

            InitRoundData();
        }
        
        private void InitRoundData()
        {
            ref var selectLeader = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            _dataWorld.CreateOneData(new RoundData {
                CurrentRound = 0,
                CurrentTurn = 1,
                CurrentPlayerID = selectLeader[0].PlayerID,
                playerOrAI = PlayerOrAI.Player
            });
        }

        public void Init()
        {
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PositionInTurnQueue == 0)
                .SelectFirstEntity();

            var playerComponent = playerEntity.GetComponent<PlayerComponent>();
            playerEntity.AddComponent(new CurrentPlayerComponent());
            
            _dataWorld.RiseEvent(new EventDistributionCard {
                TargetPlayerID = playerComponent.PlayerID,
                Count = rules.CountDropCard
            });

            if (_dataWorld.IsModuleActive<TutorialGameModule>())
            {
                TutorialAction.StartFirstPlayerRound?.Invoke();
            }
            else
            {
                UpdateUIRound();
            }
        }
        
        private void SwitchRound()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = true;
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var entitiesPlayer = _dataWorld.Select<PlayerComponent>().GetEntities();
            var countPlayers = _dataWorld.Select<PlayerComponent>().Count();

            var nextRoundPlayerID = 0;
            var nextRoundPlayerType = PlayerOrAI.None;
            foreach (var playerEntity in entitiesPlayer)
            {
                ref var componentPlayer = ref playerEntity.GetComponent<PlayerComponent>();
                componentPlayer.PositionInTurnQueue--;

                if (componentPlayer.PositionInTurnQueue < 0)
                {
                    componentPlayer.PositionInTurnQueue = countPlayers -1;
                    playerEntity.RemoveComponent<CurrentPlayerComponent>();
                }

                if (componentPlayer.PositionInTurnQueue == 0)
                {
                    playerEntity.AddComponent(new CurrentPlayerComponent());
                    nextRoundPlayerID = componentPlayer.PlayerID;
                    nextRoundPlayerType = componentPlayer.playerOrAI;
                }
            }

            roundData.CurrentPlayerID = nextRoundPlayerID;
            roundData.playerOrAI = nextRoundPlayerType;
            
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
            
            UpdateUIRound();
        }

        private async void UpdateUIRound()
        {
            var uiRound = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ChangeRoundUI;
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var playerComponent = entityPlayer.GetComponent<PlayerComponent>();
            var playerViewComponent = entityPlayer.GetComponent<PlayerViewComponent>();
            
            uiRound.NewRoundView(playerViewComponent.Avatar, playerViewComponent.Name);
            await Task.Delay(1500);

            if (playerComponent.playerOrAI == PlayerOrAI.Player)
            {
                if (entityPlayer.HasComponent<PlayerDiscardCardComponent>())
                {
                    AbilityCardAction.PlayerDiscardCard?.Invoke();
                    return;
                }
                
                if (SelectFirstBaseAction.CheckInstallFirstBase.Invoke())
                    StartTurn();  
            }
            else
            {
                RoundAction.StartTurnAI?.Invoke();
            }
        }

        private void StartTurn()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = false;
            roundData.CurrentRoundState = RoundState.Map;
            
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
            CityAction.UpdatePresencePlayerInCity?.Invoke();
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<RoundData>();
            
            RoundAction.EndCurrentTurn -= SwitchRound;
            RoundAction.StartTurn -= StartTurn;
        }
    }
}