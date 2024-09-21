using System;
using CyberNet.Core.AbilityCard;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CyberNet.Core
{
    public static class SetViewAbilityCard
    {
        public static void SetView(Transform container, AbilityCardContainer ability, BoardGameData boardGameData, CardsConfig cardConfig, bool isOneAbility = false, bool abilityIsDifferentType = false)
        {
            var boardGameConfig = boardGameData.BoardGameConfig;
            var viewCard = boardGameData.CardsViewConfig;

            if (!isOneAbility)
            {
                if (abilityIsDifferentType)
                {
                    Object.Instantiate(viewCard.IconsArrowChooseAbility, container);
                }
                else
                {
                    var abilityPlayingOnMap = AbilityCardUtilsAction.CheckAbilityIsPlayingOnMap.Invoke(ability.AbilityType);

                    if (abilityPlayingOnMap)
                        Object.Instantiate(viewCard.IconsArenaAbility, container);
                    else
                        Object.Instantiate(viewCard.IconsMapAbility, container);
                }
            }

            SetAction(container, ability, boardGameConfig, viewCard, cardConfig, isOneAbility);

            container.gameObject.SetActive(true);
        }

        private static void SetAction(Transform container, AbilityCardContainer ability, BoardGameConfig boardGameConfig, CardsViewConfigSO viewCard, CardsConfig cardConfig, bool oneAbility = false)
        {
            var abilityIsIcons = AbilityIsIconsView(ability.AbilityType);
            
            if (abilityIsIcons)
            {
                var keyIcons = ability.AbilityType.ToString();
                
                var colorCurrency = boardGameConfig.CurrencyColor[keyIcons];
                var imageCurrency = boardGameConfig.CurrencyImage[keyIcons];
                
                AddBaseAction(container, imageCurrency, ability.Count, colorCurrency, viewCard, oneAbility);
            }
            else
            {
                cardConfig.AbilityCard.TryGetValue(ability.AbilityType.ToString(), out var abilityCardConfig);
                var textCard = Object.Instantiate(viewCard.TextBaseAbility, container);

                if (ability.Count > 1 && abilityCardConfig.abilityLocMany != null)
                {
                    textCard.SetParameters(ability.Count);
                    textCard.SetText(abilityCardConfig.abilityLocMany);
                }
                else
                    textCard.SetText(abilityCardConfig.AbilityLoc);
            }
        }

        private static bool AbilityIsIconsView(AbilityType abilityType)
        {
            var isIcons = false;

            switch (abilityType)
            {
                case AbilityType.Trade:
                    isIcons = true;
                    break;
                case AbilityType.PowerPoint:
                    isIcons = true;
                    break;
                case AbilityType.KillPoint:
                    isIcons = true;
                    break;
                case AbilityType.DefencePoint:
                    isIcons = true;
                    break;
            }

            return isIcons;
        }

        private static void AddBaseAction(Transform container, Sprite sprite, int count, Color32 colorCount, CardsViewConfigSO viewCard, bool oneAbility)
        {
            if (count > 1)
            {
                var textCount = Object.Instantiate(viewCard.TextBaseAbilityCountItem, container);
                textCount.text = count.ToString();
                textCount.color = colorCount;
                if (oneAbility)
                    textCount.fontSize *= 1.4f;
            }

            var image = Object.Instantiate(viewCard.IconsBaseAbility, container);
            image.sprite = sprite;
            
            if (oneAbility)
                image.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        }
    }
}