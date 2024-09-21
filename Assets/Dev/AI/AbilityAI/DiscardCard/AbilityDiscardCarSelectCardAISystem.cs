using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard.DiscardCard;
using CyberNet.Core.EnemyTurnView;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.AI.Ability
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDiscardCarSelectCardAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float time_discard_card = 0.35f;
        private const float time_end_discard_card = 0.7f;
        
        public void PreInit()
        {
            AbilityAIAction.DiscardCardSelectCard += SelectCardToDiscard;
        }
        
        //AI выбирает какую карту сбросить
        private void SelectCardToDiscard()
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var playerDiscardComponent = playerEntity.GetComponent<PlayerEffectDiscardCardComponent>();
            var playerComponent = playerEntity.GetComponent<PlayerComponent>();
            var playerID = playerComponent.PlayerID;
            
            ref var botConfigData = ref _dataWorld.OneData<BotConfigData>();
            var cardInHandEntities = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.PlayerID == playerID)
                .GetEntities();
            
            var scoresCard = new List<ScoreCardToBuy>();
            foreach (var cardEntity in cardInHandEntities)
            {
                ref var cardComponent = ref cardEntity.GetComponent<CardComponent>();
                var scoreCard = CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_0, botConfigData)
                    + CalculateOptimalCard.CalculateCardScore(cardComponent.Ability_1, botConfigData);
                scoreCard /= cardComponent.Price;
                scoresCard.Add(new ScoreCardToBuy { GUID = cardComponent.GUID, ScoreCard = scoreCard, Cost = cardComponent.Price});
            }

            var discardCardAIComponent = new DiscardCardAIComponent{DiscardCard = new()};
            
            for (int i = 0; i < playerDiscardComponent.Count; i++)
            {
                var isMinimalScore = 100;
                var idTargetCard = 0;
                var counterCard = 0;
                
                foreach (var scoreCard in scoresCard)
                {
                    if (scoreCard.Cost < isMinimalScore)
                        idTargetCard = counterCard;

                    counterCard++;
                }

                var selectEntityCard = _dataWorld.Select<CardComponent>()
                    .With<CardHandComponent>()
                    .Where<CardComponent>(card => card.GUID == scoresCard[idTargetCard].GUID)
                    .SelectFirstEntity();

                var cardKey = selectEntityCard.GetComponent<CardComponent>().Key;
                discardCardAIComponent.DiscardCard.Add(cardKey);
                
                selectEntityCard.RemoveComponent<CardHandComponent>();
                selectEntityCard.AddComponent(new CardMoveToDiscardComponent());
                scoresCard.RemoveAt(idTargetCard);
            }

            playerEntity.AddComponent(discardCardAIComponent);
            LogicViewDiscardCard();
        }

        private void LogicViewDiscardCard()
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            ref var discardCardComponent = ref playerEntity.GetComponent<PlayerEffectDiscardCardComponent>();
            ref var discardCardAIComponent = ref playerEntity.GetComponent<DiscardCardAIComponent>();

            discardCardComponent.Count--;
            var selectDiscardCardKey = discardCardAIComponent.DiscardCard[0];
            
            EnemyTurnViewUIAction.ShowViewEnemyCardCurrentPlayer?.Invoke(EnemyTurnActionType.DiscardCard, selectDiscardCardKey);

            discardCardAIComponent.DiscardCard.Remove(selectDiscardCardKey);

            var timeEntity = _dataWorld.NewEntity();

            if (discardCardComponent.Count <= 0)
            {
                timeEntity.AddComponent(new TimeComponent {
                    Time = time_end_discard_card, Action = () => EndDiscardCard()});
            }
            else
            {
                timeEntity.AddComponent(new TimeComponent {
                    Time = time_discard_card, Action = () => LogicViewDiscardCard()});      
            }
        }

        private void EndDiscardCard()
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            
            playerEntity.RemoveComponent<PlayerEffectDiscardCardComponent>();
            playerEntity.RemoveComponent<DiscardCardAIComponent>();
            
            AnimationsMoveAtDiscardDeckAction.AnimationsMoveAtDiscardDeck?.Invoke();
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();
            AbilityAIAction.EndDiscardCard?.Invoke();
        }

        public void Destroy()
        {
            AbilityAIAction.DiscardCardSelectCard -= SelectCardToDiscard;
        }
    }
}