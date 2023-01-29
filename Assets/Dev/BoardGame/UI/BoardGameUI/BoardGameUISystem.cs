using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class BoardGameUISystem : IInitSystem, IPostRunEventSystem<EventBoardGameUpdate>
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            var gameUI = _dataWorld.OneData<BoardGameUIComponent>();
            var camera = _dataWorld.OneData<BoardGameCameraComponent>();

            var canvas = gameUI.UIGO.GetComponent<Canvas>();
            canvas.worldCamera = camera.MainCamera;
        }

        public void PostRunEvent(EventBoardGameUpdate _)
        {
            UpdatePlayerCurrency();
            UpdateStatsPlayers();
        }

        private void UpdatePlayerCurrency()
        {
            ref var actionValue = ref _dataWorld.OneData<ActionData>();
            ref var gameUI = ref _dataWorld.OneData<BoardGameUIComponent>();

            var attackValue = actionValue.TotalAttack - actionValue.SpendAttack;
            var tradeValue = actionValue.TotalTrade - actionValue.SpendTrade;

            gameUI.UIMono.SetInteractiveValue(attackValue, tradeValue);
        }

        private void UpdateStatsPlayers()
        {
            ref var playerStats = ref _dataWorld.OneData<PlayerStatsData>();
            ref var enemyStats = ref _dataWorld.OneData<EnemyStatsData>();
            ref var gameUI = ref _dataWorld.OneData<BoardGameUIComponent>().UIMono;

            gameUI.SetPlayerStats(playerStats.Influence);
            gameUI.SetEnemyStats(enemyStats.Influence);
        }
    }
}