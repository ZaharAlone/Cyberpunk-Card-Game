using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AbilityCard.DiscardCard;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.Sound;
using CyberNet.Tutorial;

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
                playerOrAI = PlayerOrAI.Player,
            });
        }

        public void Init()
        {
            var rules = _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PositionInTurnQueue == 0)
                .SelectFirstEntity().AddComponent(new CurrentPlayerComponent());
            
            var playersEntities = _dataWorld.Select<PlayerComponent>().GetEntities();

            foreach (var playerEntity in playersEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerComponent>();
                _dataWorld.RiseEvent(new EventDistributionCard {
                    TargetPlayerID = playerComponent.PlayerID,
                    Count = rules.CountDropCard
                });
            }

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

            _dataWorld.RiseEvent(new EventDistributionCard {
                TargetPlayerID = roundData.CurrentPlayerID,
                Count = rules.CountDropCard
            });

            if (roundData.playerOrAI == PlayerOrAI.Player)
            {
                CardDistributionAction.EndDistributionCard += EndDistributionCardAndStartNewRound;
            }
            else
            {
                EndDistributionCardAndStartNewRound();
            }
        }

        private void EndDistributionCardAndStartNewRound()
        {
            CardDistributionAction.EndDistributionCard -= EndDistributionCardAndStartNewRound;
            
            ref var roundData = ref _dataWorld.OneData<RoundData>();
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

            if (roundData.CurrentTurn == 1)
            {
                roundData.CurrentRound++;
                roundData.CurrentTurn = 0;
            }
            else
                roundData.CurrentTurn++;
            
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
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
            
            uiRound.NewRoundView(playerViewComponent.AvatarWithBackground, playerViewComponent.Name);
            PlayRoundSFX(playerComponent.playerOrAI == PlayerOrAI.Player);
            
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();
            await Task.Delay(1500);

            if (playerComponent.playerOrAI == PlayerOrAI.Player)
            {
                if (entityPlayer.HasComponent<PlayerEffectDiscardCardComponent>())
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

        private async void PlayRoundSFX(bool isPlayerTurn)
        {
            var soundList = _dataWorld.OneData<SoundData>().Sound;
            
            if (isPlayerTurn)
            {
                SoundAction.PlaySound?.Invoke(soundList.StartTurnPlayer);
                await Task.Delay(1500);
                SoundAction.PlaySound?.Invoke(soundList.EndTurnPlayer);
            }
            else
            {
                SoundAction.PlaySound?.Invoke(soundList.StartTurnEnemy);
                await Task.Delay(1500);
                SoundAction.PlaySound?.Invoke(soundList.EndTurnEnemy);
            }
        }

        private void StartTurn()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = false;
            roundData.CurrentGameStateMapVSArena = GameStateMapVSArena.Map;
            
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