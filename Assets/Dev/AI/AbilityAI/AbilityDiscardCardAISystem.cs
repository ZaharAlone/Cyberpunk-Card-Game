using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core.AI
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDiscardCardAISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityAIAction.DiscardCardSelectPlayer += DiscardCardSelectPlayer;
            AbilityAIAction.DiscardCardSelectCard += DiscardCardSelectCard;
        }
        
        private void DiscardCardSelectPlayer()
        {
            var playerEntities = _dataWorld.Select<PlayerComponent>()
                .Without<CurrentPlayerComponent>()
                .GetEntities();

            var targetPlayerID = 0;
            var maxVP = -1;
            
            foreach (var playerEntity in playerEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerComponent>();
                if (playerComponent.VictoryPoint < maxVP)
                    continue;
                
                targetPlayerID = playerComponent.PlayerID;
                maxVP = playerComponent.VictoryPoint;
            }
            
            var targetPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == targetPlayerID)
                .SelectFirstEntity();
            
            if (targetPlayerEntity.HasComponent<PlayerDiscardCardComponent>())
            {
                ref var playerDiscardCardComponent = ref targetPlayerEntity.GetComponent<PlayerDiscardCardComponent>();
                playerDiscardCardComponent.Count++;
            }
            else
            {
                targetPlayerEntity.AddComponent(new PlayerDiscardCardComponent {Count = 1});
            }
            
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();
        }

        private void DiscardCardSelectCard()
        {
            Debug.LogError("Discard card enemy");
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var playerDiscardComponent = playerEntity.GetComponent<PlayerDiscardCardComponent>();
            
            ref var botConfigData = ref _dataWorld.OneData<BotConfigData>();
            var cardInHand = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .GetEntities();
            
            var scoresCard = new List<ScoreCardToBuy>();
            foreach (var cardEntity in cardInHand)
            {
                ref var cardComponent = ref cardEntity.GetComponent<CardComponent>();
                var scoreCard = CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_0, botConfigData)
                    + CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_1, botConfigData)
                    + CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_2, botConfigData);
                scoreCard /= cardComponent.Price;
                scoresCard.Add(new ScoreCardToBuy { GUID = cardComponent.GUID, ScoreCard = scoreCard, Cost = cardComponent.Price});
            }
            
            for (int i = 0; i < playerDiscardComponent.Count; i++)
            {
                var isMinimalScore = 100;
                var idTargetCard = 0;
                var counterCard = 0;
                
                foreach (var scoreCard in scoresCard)
                {
                    if (scoreCard.Cost < isMinimalScore)
                    {
                        idTargetCard = counterCard;
                    }

                    counterCard++;
                }

                var selectEntityCard = _dataWorld.Select<CardComponent>()
                    .With<CardHandComponent>()
                    .Where<CardComponent>(card => card.GUID == scoresCard[idTargetCard].GUID)
                    .SelectFirstEntity();

                selectEntityCard.RemoveComponent<CardHandComponent>();
                selectEntityCard.AddComponent(new CardMoveToDiscardComponent());
                scoresCard.RemoveAt(idTargetCard);
            }

            playerEntity.RemoveComponent<PlayerDiscardCardComponent>();
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();
        }
    }
}