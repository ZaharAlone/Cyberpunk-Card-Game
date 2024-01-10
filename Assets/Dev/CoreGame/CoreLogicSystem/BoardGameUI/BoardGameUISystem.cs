using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using CyberNet.Global.GameCamera;
using UnityEngine;

namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class BoardGameUISystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI += UpdateStatsPlayersPassport;
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI += ViewPlayerPassport;
            BoardGameUIAction.UpdateStatsPlayersCurrency += UpdatePlayerCurrency;
            BoardGameUIAction.UpdateCountCardInHand += UpdateCountCard;
            RoundAction.EndCurrentTurn += ViewPlayerPassport;
        }

        public void Init()
        {
            UpdateCountCard();
            ViewPlayerPassport();
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
                    ShowMainPassportPlayer(playerComponent, playerViewComponent);
                }
                else
                {
                    var countDiscardCard = 0;
                    if (entity.HasComponent<PlayerDiscardCardComponent>())
                    {
                        countDiscardCard = entity.GetComponent<PlayerDiscardCardComponent>().Count;
                    }
                    ShowLeftPassportPlayer(playerComponent, playerViewComponent, countDiscardCard);
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

        private void ShowMainPassportPlayer(PlayerComponent playerComponent, PlayerViewComponent playerViewComponent)
        {
            ref var coreUIHud = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono;
            coreUIHud.SetMainViewPassportNameAvatar(playerViewComponent.Name, playerViewComponent.Avatar);
            coreUIHud.SetMainPassportViewStats(playerComponent.UnitCount, playerComponent.VictoryPoint, playerComponent.UnitAgentCountInHand);
        }

        private void ShowLeftPassportPlayer(PlayerComponent playerComponent, PlayerViewComponent playerViewComponent, int countDiscardCard)
        {
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            ref var enemyPassport = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.EnemyPassports;
            
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetPlayerID(playerComponent.PlayerID);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetAvatar(playerViewComponent.Avatar);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetStats(playerComponent.UnitCount);
            cityVisual.UnitDictionary.TryGetValue(playerViewComponent.KeyCityVisual, out var playerUnitVisual);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetStatsColor(playerUnitVisual.ColorUnit);
            
            enemyPassport[playerComponent.PositionInTurnQueue - 1].DiscardCardStatus(countDiscardCard);
        }

        private void UpdatePlayerCurrency()
        {
            ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>();

            var tradeValue = actionValue.TotalTrade - actionValue.SpendTrade;

            gameUI.BoardGameUIMono.TraderowMono.SetTradeValue(tradeValue);
        }
        
        private void UpdateStatsPlayersPassport()
        {
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            var roundData = _dataWorld.OneData<RoundData>();
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player=> player.PlayerID == roundData.CurrentPlayerID)
                .SelectFirstEntity();

            ref var playerComponent = ref entityPlayer.GetComponent<PlayerComponent>();
            gameUI.CoreHudUIMono.SetMainPassportViewStats(playerComponent.UnitCount, playerComponent.Cyberpsychosis, playerComponent.UnitAgentCountInHand);
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
            
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI -= UpdateStatsPlayersPassport;
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI -= ViewPlayerPassport;
            BoardGameUIAction.UpdateStatsPlayersCurrency -= UpdatePlayerCurrency;
            BoardGameUIAction.UpdateCountCardInHand -= UpdateCountCard;
            RoundAction.EndCurrentTurn -= ViewPlayerPassport;
        }
    }
}