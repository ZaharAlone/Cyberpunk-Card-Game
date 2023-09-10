using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using CyberNet.Core.ActionCard;
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
            ViewPlayerPassport();
        }

        public void PostRunEvent(EventBoardGameUpdate _)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            UpdateCountCard();
        }

        private void ViewPlayerPassport()
        {
            ref var coreUIHud = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono;
            var entitiesPlayer = _dataWorld.Select<PlayerComponent>()
                                                         .With<PlayerViewComponent>()
                                                         .GetEntities();

            var counter = 0;
            foreach (var entity in entitiesPlayer)
            {
                var playerComponent = entity.GetComponent<PlayerComponent>();
                var playerViewComponent = entity.GetComponent<PlayerViewComponent>();

                if (playerComponent.PositionInTurnQueue == 0)
                {
                    ShowDownPassportPlayer(playerComponent, playerViewComponent);
                }
                else
                {
                    ShowLeftPassportPlayer(playerComponent, playerViewComponent);
                }

                counter++;
            }

            for (int i = 0; i < coreUIHud.EnemyPassports.Count; i++)
            {
                if ((counter - 1) > i)
                    continue;
                coreUIHud.EnemyPassports[i].gameObject.SetActive(false);
            }
        }

        private void ShowDownPassportPlayer(PlayerComponent playerComponent, PlayerViewComponent playerViewComponent)
        {
            ref var coreUIHud = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono;
            coreUIHud.SetMainViewPassportNameAvatar(playerViewComponent.Name, playerViewComponent.Avatar);
            coreUIHud.SetMainPassportViewStats(playerComponent.UnitCount, playerComponent.Cyberpsychosis);
        }

        private void ShowLeftPassportPlayer(PlayerComponent playerComponent, PlayerViewComponent playerViewComponent)
        {
            ref var enemyPassport = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.EnemyPassports;
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetAvatar(playerViewComponent.Avatar);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetStats(playerComponent.UnitCount);
        }

        private void UpdatePlayerCurrency()
        {
            ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>();

            var attackValue = actionValue.TotalAttack - actionValue.SpendAttack;
            var tradeValue = actionValue.TotalTrade - actionValue.SpendTrade;

            gameUI.BoardGameUIMono.CoreHudUIMono.SetInteractiveValue(attackValue, tradeValue);
        }
        
        private void UpdateStatsPlayersPassport()
        {
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            var roundData = _dataWorld.OneData<RoundData>();
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player=> player.PlayerID == roundData.CurrentPlayerID)
                .SelectFirstEntity();

            ref var playerComponent = ref entityPlayer.GetComponent<PlayerComponent>();
            gameUI.CoreHudUIMono.SetMainPassportViewStats(playerComponent.UnitCount, playerComponent.Cyberpsychosis);
        }


        private void UpdateCountCard()
        {
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            var roundData = _dataWorld.OneData<RoundData>();
            var discardCard = _dataWorld.Select<CardComponent>()
                                                .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
                                                .With<CardDiscardComponent>()
                                                .Count();
            var drawCard = _dataWorld.Select<CardComponent>()
                                             .Where<CardComponent>(card => card.PlayerID == roundData.CurrentPlayerID)
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