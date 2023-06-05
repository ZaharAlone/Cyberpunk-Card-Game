using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CyberNet.Core
{
    public static class SetViewAbilityCard
    {
        public static void SetView(Transform container, AbilityCard ability, BoardGameConfig boardGameConfig, bool chooseAbility = false, bool oneAbility = false)
        {
            if (chooseAbility)
                Object.Instantiate(boardGameConfig.IconsArrowChooseAbility, container);

            if (ability.Condition != AbilityCondition.None)
                SetConfition(container, ability, boardGameConfig);

            SetAction(container, ability, boardGameConfig, oneAbility);

            container.gameObject.SetActive(true);
        }

        private static void SetConfition(Transform container, AbilityCard ability, BoardGameConfig boardGameConfig)
        {
            switch (ability.Condition)
            {
                case AbilityCondition.Corporates:
                    SetDoubleNations(container, boardGameConfig, "Corporates");
                    break;
                case AbilityCondition.Guns:
                    SetDoubleNations(container, boardGameConfig, "Guns");
                    break;
                case AbilityCondition.Nomads:
                    SetDoubleNations(container, boardGameConfig, "Nomads");
                    break;
                case AbilityCondition.Netrunners:
                    SetDoubleNations(container, boardGameConfig, "Netrunners");
                    break;
                case AbilityCondition.Destroy:
                    var textDestroy = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDestroy.text = "Destroy Card";
                    break;
            }

            var imageArrow = Object.Instantiate(boardGameConfig.IconsArrowConditionAbility, container);
        }

        private static void SetDoubleNations(Transform container, BoardGameConfig boardGameConfig, string nationsName)
        {
            boardGameConfig.NationsImage.TryGetValue(nationsName, out var icons);
            var im_1 = Object.Instantiate(boardGameConfig.IconsBaseAbility, container);
            var im_2 = Object.Instantiate(boardGameConfig.IconsBaseAbility, container);
            im_1.sprite = icons;
            im_2.sprite = icons;
        }

        private static void SetAction(Transform container, AbilityCard ability, BoardGameConfig boardGameConfig, bool oneAbility = false)
        {
            switch (ability.Action)
            {
                case AbilityAction.Attack:
                    boardGameConfig.CurrencyImage.TryGetValue("Attack", out var imAttack);
                    AddBaseAction(container, imAttack, ability.Count, boardGameConfig.ColorAttackText, boardGameConfig, oneAbility);
                    break;
                case AbilityAction.Trade:
                    boardGameConfig.CurrencyImage.TryGetValue("Trade", out var imTrade);
                    AddBaseAction(container, imTrade, ability.Count, boardGameConfig.ColorTradeText, boardGameConfig, oneAbility);
                    break;
                case AbilityAction.Influence:
                    boardGameConfig.CurrencyImage.TryGetValue("Influence", out var imInfluence);
                    AddBaseAction(container, imInfluence, ability.Count, boardGameConfig.ColorInfluenceText, boardGameConfig, oneAbility);
                    break;
                case AbilityAction.DrawCard:
                    var textDraw = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDraw.text = "Draw Card";
                    break;
                case AbilityAction.DiscardCardEnemy:
                    var textDiscard = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDiscard.text = "Discard Card Enemy";
                    break;
                case AbilityAction.DestroyCard:
                    var textDestroy = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDestroy.text = "Destroy Card";
                    break;
                case AbilityAction.DownCyberpsychosisEnemy:
                    var textDownCyb = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDownCyb.text = "Down Cyberpsychosis";
                    break;
                case AbilityAction.CloneCard:
                    var textClone = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textClone.text = "Clone card in hand";
                    break;
                case AbilityAction.NoiseCard:
                    var textNoise = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textNoise.text = "Add Card Noise Enemy";
                    break;
                case AbilityAction.ThiefCard:
                    var textThief = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textThief.text = "Thief Random Card";
                    break;
                case AbilityAction.DestroyTradeCard:
                    var textDestroyTrade = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDestroyTrade.text = "Destroy Trade Card";
                    break;
                case AbilityAction.DestroyEnemyBase:
                    var textDestroyBase = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDestroyBase.text = "Destroy Enemy Base";
                    break;
            }
        }

        private static void AddBaseAction(Transform container, Sprite sprite, int count, Color32 colorCount, BoardGameConfig boardGameConfig, bool oneAbility)
        {
            if (count > 1)
            {
                var textCount = Object.Instantiate(boardGameConfig.TextBaseAbilityCountItem, container);
                textCount.text = count.ToString();
                textCount.color = colorCount;
                if (oneAbility)
                    textCount.fontSize *= 1.4f;
            }

            var image = Object.Instantiate(boardGameConfig.IconsBaseAbility, container);
            image.sprite = sprite;
            
            if (oneAbility)
                image.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        }
    }
}