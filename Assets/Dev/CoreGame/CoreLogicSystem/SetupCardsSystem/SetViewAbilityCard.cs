using CyberNet.Core.AbilityCard;
using UnityEngine;

namespace CyberNet.Core
{
    public static class SetViewAbilityCard
    {
        public static void SetView(Transform container, AbilityCardContainer ability, BoardGameConfig boardGameConfig, CardsConfig cardConfig, bool chooseAbility = false, bool oneAbility = false)
        {
            if (chooseAbility)
                Object.Instantiate(boardGameConfig.IconsArrowChooseAbility, container);

            if (ability.Condition != AbilityCondition.None)
                SetConfition(container, ability, boardGameConfig);

            SetAction(container, ability, boardGameConfig, cardConfig, oneAbility);

            container.gameObject.SetActive(true);
        }

        private static void SetConfition(Transform container, AbilityCardContainer ability, BoardGameConfig boardGameConfig)
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
            im_1.sprite = icons;
        }

        private static void SetAction(Transform container, AbilityCardContainer ability, BoardGameConfig boardGameConfig, CardsConfig cardConfig, bool oneAbility = false)
        {
            if (ability.AbilityType == AbilityType.Trade)
            {
                boardGameConfig.CurrencyImage.TryGetValue("Trade", out var imTrade);
                AddBaseAction(container, imTrade, ability.Count, boardGameConfig.ColorTradeText, boardGameConfig, oneAbility);
            }
            else
            {
                cardConfig.AbilityCard.TryGetValue(ability.AbilityType.ToString(), out var abilityCardConfig);
                var textCard = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                
                if (ability.Count > 1 && abilityCardConfig.AbilityLocMultiple != null)
                    textCard.text = I2.Loc.LocalizationManager.GetTranslation(abilityCardConfig.AbilityLocMultiple);
                else
                    textCard.text = I2.Loc.LocalizationManager.GetTranslation(abilityCardConfig.AbilityLoc);
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