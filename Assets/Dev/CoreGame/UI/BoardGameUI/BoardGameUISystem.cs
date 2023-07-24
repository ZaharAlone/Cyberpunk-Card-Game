using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using CyberNet.Core.Ability;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class BoardGameUISystem : IPreInitSystem, IInitSystem, IPostRunEventSystem<EventBoardGameUpdate>
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            InitCameraCanvas();
            
            BoardGameUIAction.UpdateStatsPlayerUI += UpdateStatsPlayers;
        }
        
        private void InitCameraCanvas()
        {
            var gameUI = _dataWorld.OneData<UIData>();
            var camera = _dataWorld.OneData<BoardGameCameraComponent>();

            var canvas = gameUI.UIGO.GetComponent<Canvas>();
            canvas.worldCamera = camera.MainCamera;
        }
        public void Init()
        {
            UpdateView();
        }

        public void PostRunEvent(EventBoardGameUpdate _)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            UpdatePlayerCurrency();
            UpdateViewPassport();
            UpdateCountCard();
        }

        private void UpdatePlayerCurrency()
        {
            ref var actionValue = ref _dataWorld.OneData<AbilityData>();
            ref var gameUI = ref _dataWorld.OneData<UIData>();

            var attackValue = actionValue.TotalAttack - actionValue.SpendAttack;
            var tradeValue = actionValue.TotalTrade - actionValue.SpendTrade;

            gameUI.UIMono.SetInteractiveValue(attackValue, tradeValue);
        }

        private void UpdateStatsPlayers()
        {
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            ref var player1Stats = ref _dataWorld.OneData<Player1StatsData>();
            ref var player2Stats = ref _dataWorld.OneData<Player2StatsData>();
            ref var gameUI = ref _dataWorld.OneData<UIData>().UIMono;

            if (viewPlayer.PlayerView == PlayerEnum.Player1)
            {
                gameUI.SetViewDownTableStats(player1Stats.HP, player1Stats.Cyberpsychosis);
                gameUI.SetViewUpTableStats(player2Stats.HP, player2Stats.Cyberpsychosis);
            }
            else
            {
                gameUI.SetViewDownTableStats(player2Stats.HP, player2Stats.Cyberpsychosis);
                gameUI.SetViewUpTableStats(player1Stats.HP, player1Stats.Cyberpsychosis);
            }
        }

        private void UpdateViewPassport()
        {
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var leadersData = _dataWorld.OneData<LeadersViewData>();
            ref var player1View = ref _dataWorld.OneData<Player1ViewData>();
            ref var player2View = ref _dataWorld.OneData<Player2ViewData>();
            ref var gameUI = ref _dataWorld.OneData<UIData>().UIMono;

            leadersData.LeadersView.TryGetValue(player1View.AvatarKey, out var avatarPlayer1);
            leadersData.LeadersView.TryGetValue(player2View.AvatarKey, out var avatarPlayer2);
            
            if (viewPlayer.PlayerView == PlayerEnum.Player1)
            {
                gameUI.SetViewNameAvatarDownTable(player1View.Name, avatarPlayer1);
                gameUI.SetViewNameAvatarUpTable(player2View.Name, avatarPlayer2);
            }
            else
            {
                gameUI.SetViewNameAvatarDownTable(player2View.Name, avatarPlayer2);
                gameUI.SetViewNameAvatarUpTable(player1View.Name, avatarPlayer1);
            }
        }

        private void UpdateCountCard()
        {
            ref var gameUI = ref _dataWorld.OneData<UIData>().UIMono;
            ref var viewPlayer = ref _dataWorld.OneData<ViewPlayerData>();
            var discardCardsPlayer1 = _dataWorld.Select<CardComponent>()
                                                .Where<CardComponent>(card => card.Player == PlayerEnum.Player1)
                                                .With<CardDiscardComponent>()
                                                .Count();
            var drawCardsPlayer1 = _dataWorld.Select<CardComponent>()
                                             .Where<CardComponent>(card => card.Player == PlayerEnum.Player1)
                                             .With<CardDrawComponent>()
                                             .Count();
            var discardCardsPlayer2 = _dataWorld.Select<CardComponent>()
                                                .Where<CardComponent>(card => card.Player == PlayerEnum.Player2)
                                                .With<CardDiscardComponent>()
                                                .Count();
            var drawCardsPlayer2 = _dataWorld.Select<CardComponent>()
                                             .Where<CardComponent>(card => card.Player == PlayerEnum.Player2)
                                             .With<CardDrawComponent>()
                                             .Count();

            if (viewPlayer.PlayerView == PlayerEnum.Player1)
                gameUI.SetCountCard(discardCardsPlayer1, drawCardsPlayer1, discardCardsPlayer2, drawCardsPlayer2);
            else
                gameUI.SetCountCard(discardCardsPlayer2, drawCardsPlayer2, discardCardsPlayer1, drawCardsPlayer1);
        }
    }
}