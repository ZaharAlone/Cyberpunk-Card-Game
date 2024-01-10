using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using UnityEngine;

namespace CyberNet.Core.AI
{
    public static class CalculateOptimalCard
    {
        public static float CalculateCardScore(AbilityCardContainer abilityCard, BotConfigData botConfigData)
        {
            if (abilityCard.AbilityType == AbilityType.None)
                return 0f;
            
            var value = 0f;
            var isFindConfig = botConfigData.BotScoreCard.TryGetValue(abilityCard.AbilityType.ToString(), out var multValueAction);

            //TODO: Заглушка пока еще не все конфиги заданы для карт
            if (!isFindConfig)
                return value;
            switch (abilityCard.AbilityType)
            {
                case AbilityType.Attack:
                    value = multValueAction * abilityCard.Count;
                    break;
                case AbilityType.Trade:
                    value = multValueAction * abilityCard.Count;
                    break;
                case AbilityType.DrawCard:
                    value = multValueAction;
                    break;
                case AbilityType.DestroyCard:
                    value = multValueAction;
                    break;
                case AbilityType.CloneCard:
                    value = multValueAction;
                    break;
            }
            
            return value;
        }
        
        public static List<string> FindOptimalPurchase(List<ScoreCardToBuy> scoreCardToBuy, int budget)
        {
            var countCard = scoreCardToBuy.Count;
            var dp = new float[countCard + 1, budget + 1];

            for (int i = 0; i <= countCard; i++)
            {
                for (int w = 0; w <= budget; w++)
                {
                    if (i == 0 || w == 0)
                    {
                        dp[i, w] = 0;
                    }
                    else if (scoreCardToBuy[i - 1].Cost <= w)
                    {
                        dp[i, w] = Mathf.Max(scoreCardToBuy[i - 1].ScoreCard + dp[i - 1, w - scoreCardToBuy[i - 1].Cost], dp[i - 1, w]);
                    }
                    else
                    {
                        dp[i, w] = dp[i - 1, w];
                    }
                }
            }

            var selectedCards = new List<string>();
            int remainingBudget = budget;
            for (int i = countCard; i > 0 && remainingBudget > 0; i--)
            {
                if (dp[i, remainingBudget] != dp[i - 1, remainingBudget])
                {
                    selectedCards.Add(scoreCardToBuy[i - 1].GUID);
                    remainingBudget -= scoreCardToBuy[i - 1].Cost;
                }
            }

            return selectedCards;
        }
    }
}