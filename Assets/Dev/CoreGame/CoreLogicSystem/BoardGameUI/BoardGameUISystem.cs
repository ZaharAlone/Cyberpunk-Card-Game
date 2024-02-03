using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using CyberNet.Global;
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
            
            var countPlayers = _dataWorld.Select<PlayerComponent>().Count();

            //Check need switch position player
            var switchPositionPassport = false;
            foreach (var entity in entitiesPlayer)
            {
                var playerComponent = entity.GetComponent<PlayerComponent>();
                
                if (playerComponent.playerOrAI == PlayerOrAI.Player && playerComponent.PositionInTurnQueue == 0)
                {
                    switchPositionPassport = true;
                    break;
                }
            }

            //Switch positions players
            if (switchPositionPassport)
            {
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
                        ShowLeftPassportPlayer(playerComponent, playerViewComponent);
                    }
                }
            }
            
            //Enable/Disable effect current player
            foreach (var entity in entitiesPlayer)
            {
                var playerComponent = entity.GetComponent<PlayerComponent>();
                
                if (playerComponent.playerOrAI == PlayerOrAI.Player)
                {
                    coreUIHud.EnableMainPlayerCurrentRound(playerComponent.PositionInTurnQueue == 0);
                }
                else
                {
                    coreUIHud.EnableLeftPlayerCurrentRound(playerComponent.PositionInTurnQueue == 0, playerComponent.PlayerID);
                }
            }

            for (int i = 0; i < coreUIHud.EnemyPassports.Count; i++)
            {
                if ((countPlayers - 1) > i)
                    continue;
                coreUIHud.EnemyPassports[i].gameObject.SetActive(false);
            }
        }

        private void ShowMainPassportPlayer(PlayerComponent playerComponent, PlayerViewComponent playerViewComponent)
        {
            ref var coreUIHud = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono;
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            cityVisual.UnitDictionary.TryGetValue(playerViewComponent.KeyCityVisual, out var playerUnitVisual);
            
            coreUIHud.SetMainViewPassportNameAvatar(playerViewComponent.Name, playerViewComponent.Avatar, playerUnitVisual.IconsUnit, playerUnitVisual.ColorUnit);
            coreUIHud.SetMainPassportViewStats(playerComponent.UnitCount, playerComponent.CurrentCountControlTerritory, playerComponent.CurrentCountControlBase);
        }

        private void ShowLeftPassportPlayer(PlayerComponent playerComponent, PlayerViewComponent playerViewComponent)
        {
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            ref var enemyPassport = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.EnemyPassports;

            var countCardHand = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerComponent.PlayerID)
                .Count();
            
            var countCardDiscard = _dataWorld.Select<CardComponent>()
                .With<CardDiscardComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerComponent.PlayerID)
                .Count();
            
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetPlayerID(playerComponent.PlayerID);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetStats(countCardHand, countCardDiscard, playerComponent.UnitCount);
            
            cityVisual.UnitDictionary.TryGetValue(playerViewComponent.KeyCityVisual, out var playerUnitVisual);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetViewPlayer(playerViewComponent.Avatar, playerViewComponent.Name, playerUnitVisual.IconsUnit, playerUnitVisual.ColorUnit);
            
            /*
            var countDiscardCard = 0;
            if (entity.HasComponent<PlayerDiscardCardComponent>())
            {
                countDiscardCard = entity.GetComponent<PlayerDiscardCardComponent>().Count;
            }
            enemyPassport[playerComponent.PositionInTurnQueue - 1].DiscardCardStatus(countDiscardCard);*/
        }

        private void UpdatePlayerCurrency()
        {
            ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>();

            var tradeValue = actionValue.TotalTrade - actionValue.SpendTrade;

            // TODO временно всегда воспроизводим эффект
            gameUI.BoardGameUIMono.TraderowMono.SetTradeValue(tradeValue, true);
        }
        
        private void UpdateStatsPlayersPassport()
        {
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            var roundData = _dataWorld.OneData<RoundData>();
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player=> player.PlayerID == roundData.CurrentPlayerID)
                .SelectFirstEntity();

            ref var playerComponent = ref entityPlayer.GetComponent<PlayerComponent>();
            gameUI.CoreHudUIMono.SetMainPassportViewStats(playerComponent.UnitCount, playerComponent.CurrentCountControlTerritory, playerComponent.CurrentCountControlBase);
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