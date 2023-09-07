using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using CyberNet.Core.ActionCard;
using CyberNet.Global;
using CyberNet.Global.GameCamera;
using UnityEngine;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class BoardGameUISystem : IPreInitSystem, IInitSystem, IPostRunEventSystem<EventBoardGameUpdate>, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InitCameraCanvas();
            
            BoardGameUIAction.UpdateStatsPlayersPassportUI += UpdateStatsPlayersPassport;
            BoardGameUIAction.UpdateStatsPlayersCurrency += UpdatePlayerCurrency;
        }
        
        private void InitCameraCanvas()
        {
            var gameUI = _dataWorld.OneData<CoreGameUIData>();
            var camera = _dataWorld.OneData<GameCameraData>();

            var canvas = gameUI.UIGO.GetComponent<Canvas>();
            canvas.worldCamera = camera.MainCamera;
        }
        public void Init()
        {
            UpdateView();
            FirstInitPassport();
        }

        public void PostRunEvent(EventBoardGameUpdate _)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            UpdateViewPassport();
            UpdateCountCard();
        }
        
        private void FirstInitPassport()
        {
            ref var coreUIHud = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono;
            ref var selectPlayerData = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            
            
            
        }

        private void UpdatePlayerCurrency()
        {
            ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>();

            var attackValue = actionValue.TotalAttack - actionValue.SpendAttack;
            var tradeValue = actionValue.TotalTrade - actionValue.SpendTrade;

            gameUI.BoardGameUIMono.CoreHudUIMono.SetInteractiveValue(attackValue, tradeValue);
        }

        //TODO: вернуть
        private void UpdateStatsPlayersPassport()
        {
            /*
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            ref var player1Stats = ref _dataWorld.OneData<PlayerStatsComponent>();
            ref var player2Stats = ref _dataWorld.OneData<Player2StatsData>();
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;

            //TODO: старый код
            if (viewPlayer.PlayerView == PlayerEnum.Player1)
            {
                gameUI.CoreHudUIMono.SetViewDownTableStats(player1Stats.HP, player1Stats.Cyberpsychosis);
                //gameUI.CoreHudUIMono.SetViewUpTableStats(player2Stats.HP, player2Stats.Cyberpsychosis);
            }
            else
            {
                gameUI.CoreHudUIMono.SetViewDownTableStats(player2Stats.HP, player2Stats.Cyberpsychosis);
                //gameUI.CoreHudUIMono.SetViewUpTableStats(player1Stats.HP, player1Stats.Cyberpsychosis);
            }*/
        }

        private void UpdateViewPassport()
        {/*
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var leadersData = _dataWorld.OneData<LeadersViewData>();
            
            //ref var playerComponent = ref _dataWorld.OneData<PlayerComponent>();
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;

            leadersData.LeadersView.TryGetValue(playerComponent.AvatarKey, out var avatarPlayer1);
            leadersData.LeadersView.TryGetValue(player2View.AvatarKey, out var avatarPlayer2);
            
            if (viewPlayer.PlayerView == PlayerEnum.Player1)
            {
                gameUI.CoreHudUIMono.SetViewNameAvatarDownTable(playerComponent.Name, avatarPlayer1);
                //gameUI.CoreHudUIMono.SetViewNameAvatarUpTable(player2View.Name, avatarPlayer2);
            }
            else
            {
                gameUI.CoreHudUIMono.SetViewNameAvatarDownTable(player2View.Name, avatarPlayer2);
                //gameUI.CoreHudUIMono.SetViewNameAvatarUpTable(player1View.Name, avatarPlayer1);
            }*/
        }

        private void UpdateCountCard()
        {
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            var viewPlayer = _dataWorld.OneData<CurrentPlayerViewScreenData>();
            var discardCard = _dataWorld.Select<CardComponent>()
                                                .Where<CardComponent>(card => card.PlayerID == viewPlayer.CurrentPlayerID)
                                                .With<CardDiscardComponent>()
                                                .Count();
            var drawCard = _dataWorld.Select<CardComponent>()
                                             .Where<CardComponent>(card => card.PlayerID == viewPlayer.CurrentPlayerID)
                                             .With<CardDrawComponent>()
                                             .Count();

            gameUI.CoreHudUIMono.SetCountCard(discardCard, drawCard);
        }

        public void Destroy()
        {
            _dataWorld.RemoveOneData<CoreGameUIData>();
        }
    }
}