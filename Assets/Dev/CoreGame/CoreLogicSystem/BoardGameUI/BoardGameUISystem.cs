using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.AbilityCard.DiscardCard;
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
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI += UpdateStatsMainPlayerPassport;
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI += UpdateViewAllPlayersPassport;
            BoardGameUIAction.UpdateStatsPlayersCurrency += UpdatePlayerCurrency;
            BoardGameUIAction.UpdateCountCardInHand += UpdateCountCard;
            BoardGameUIAction.ControlVFXCurrentPlayerArena += ControlVFXCurrentPlayerArena;
        }

        public void Init()
        {
            UpdateCountCard();
            UpdateViewAllPlayersPassport();
        }

        private void UpdateViewAllPlayersPassport()
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
                        ShowMainPassportPlayer(entity);
                    else
                        ShowLeftPassportPlayer(playerComponent, playerViewComponent);
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

        private void ShowMainPassportPlayer(Entity entityPlayer)
        {
            var playerComponent = entityPlayer.GetComponent<PlayerComponent>();
            var playerViewComponent = entityPlayer.GetComponent<PlayerViewComponent>();
            
            var coreUIHud = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono;
            var cityVisual = _dataWorld.OneData<BoardGameData>().CitySO;
            cityVisual.UnitDictionary.TryGetValue(playerViewComponent.KeyCityVisual, out var playerUnitVisual);
            var victoryPointToFinishGame = _dataWorld.OneData<BoardGameData>().BoardGameRule.VictoryPointToFinishGame;

            coreUIHud.SetMainViewPassportNameAvatar(playerViewComponent.Name, playerViewComponent.Avatar, playerUnitVisual.IconsUnit, playerUnitVisual.ColorUnit);
            
            var countDiscardCard = 0;

            if (entityPlayer.HasComponent<PlayerEffectDiscardCardComponent>())
                countDiscardCard = entityPlayer.GetComponent<PlayerEffectDiscardCardComponent>().Count;
            
            coreUIHud.SetMainPassportViewStats(playerComponent.UnitCount, playerComponent.VictoryPoint, victoryPointToFinishGame, countDiscardCard);
        }

        private void ShowLeftPassportPlayer(PlayerComponent playerComponent, PlayerViewComponent playerViewComponent)
        {
            var cityVisual = _dataWorld.OneData<BoardGameData>().CitySO;
            ref var enemyPassport = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.EnemyPassports;
            var victoryPointToFinishGame = _dataWorld.OneData<BoardGameData>().BoardGameRule.VictoryPointToFinishGame;
            
            var countCardHandAndDeck = _dataWorld.Select<CardComponent>()
                .Without<CardDiscardComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerComponent.PlayerID)
                .Count();
            
            var countCardDiscard = _dataWorld.Select<CardComponent>()
                .With<CardDiscardComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerComponent.PlayerID)
                .Count();
            
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetPlayerID(playerComponent.PlayerID);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetStats(countCardHandAndDeck, countCardDiscard, playerComponent.UnitCount);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetCountVictoryPoint(playerComponent.VictoryPoint, victoryPointToFinishGame);
            
            cityVisual.UnitDictionary.TryGetValue(playerViewComponent.KeyCityVisual, out var playerUnitVisual);
            enemyPassport[playerComponent.PositionInTurnQueue - 1].SetViewPlayer(playerViewComponent.Avatar, playerViewComponent.Name, playerUnitVisual.IconsUnit, playerUnitVisual.ColorUnit);

            //Show count discard card status
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerComponent.PlayerID)
                .SelectFirstEntity();
            
            var countDiscardCard = 0;
            if (playerEntity.HasComponent<PlayerEffectDiscardCardComponent>())
            {
                countDiscardCard = playerEntity.GetComponent<PlayerEffectDiscardCardComponent>().Count;
            }
            enemyPassport[playerComponent.PositionInTurnQueue - 1].DiscardCardStatusLeftPlayer(countDiscardCard);
        }

        private void UpdatePlayerCurrency()
        {
            ref var actionValue = ref _dataWorld.OneData<ActionCardData>();
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>();

            var tradeValue = actionValue.TotalTrade - actionValue.SpendTrade;

            gameUI.BoardGameUIMono.TraderowMono.SetTradeValue(tradeValue);
        }

        private void UpdateStatsMainPlayerPassport()
        {
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            var roundData = _dataWorld.OneData<RoundData>();
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player=> player.PlayerID == roundData.CurrentPlayerID)
                .SelectFirstEntity();
            var victoryPointToFinishGame = _dataWorld.OneData<BoardGameData>().BoardGameRule.VictoryPointToFinishGame;

            ref var playerComponent = ref entityPlayer.GetComponent<PlayerComponent>();
            var countDiscardCard = 0;

            if (entityPlayer.HasComponent<PlayerEffectDiscardCardComponent>())
                countDiscardCard = entityPlayer.GetComponent<PlayerEffectDiscardCardComponent>().Count;
            
            gameUI.CoreHudUIMono.SetMainPassportViewStats(playerComponent.UnitCount, playerComponent.VictoryPoint, victoryPointToFinishGame, countDiscardCard);
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

        private void ControlVFXCurrentPlayerArena(bool status)
        {
            var coreUIHud = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono;
            coreUIHud.EnableMainPlayerCurrentRound(status);
        }
        
        public void Destroy()
        {
            _dataWorld.RemoveOneData<CoreGameUIData>();
            
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI -= UpdateStatsMainPlayerPassport;
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI -= UpdateViewAllPlayersPassport;
            BoardGameUIAction.UpdateStatsPlayersCurrency -= UpdatePlayerCurrency;
            BoardGameUIAction.UpdateCountCardInHand -= UpdateCountCard;
            BoardGameUIAction.ControlVFXCurrentPlayerArena -= ControlVFXCurrentPlayerArena;
            RoundAction.EndCurrentTurn -= UpdateViewAllPlayersPassport;
        }
    }
}