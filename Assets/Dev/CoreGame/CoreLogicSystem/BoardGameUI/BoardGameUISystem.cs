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