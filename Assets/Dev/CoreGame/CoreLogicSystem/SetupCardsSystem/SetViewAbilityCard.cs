using CyberNet.Core.AbilityCard;
using UnityEngine;

namespace CyberNet.Core
{
    public static class SetViewAbilityCard
    {
        public static void SetView(Transform container, AbilityCardContainer ability, BoardGameData boardGameData, CardsConfig cardConfig, bool chooseAbility = false, bool abilityIsDifferentType = false, bool oneAbility = false)
        {
            var boardGameConfig = boardGameData.BoardGameConfig;
            var viewCard = boardGameData.CardsViewConfig;

            if (chooseAbility)
            {
                if (abilityIsDifferentType)
                {
                    var checkAbilityPlayingOnlyArena = AbilityCardUtilsAction.CheckAbilityIsPlayingOnlyArena.Invoke(ability.AbilityType);
                    var checkAbilityPlayingOnlyMap = AbilityCardUtilsAction.CheckAbilityIsPlayingOnlyMap.Invoke(ability.AbilityType);

                    if (checkAbilityPlayingOnlyArena)
                        Object.Instantiate(viewCard.IconsArenaAbility, container);
                    
                    if (checkAbilityPlayingOnlyMap)
                        Object.Instantiate(viewCard.IconsMapAbility, container);
                }
                else
                {
                    Object.Instantiate(viewCard.IconsArrowChooseAbility, container);
                }
            }

            if (ability.Condition != AbilityCondition.None)
                SetCondition(container, ability, boardGameConfig, viewCard);

            SetAction(container, ability, boardGameConfig, viewCard, cardConfig, oneAbility);

            container.gameObject.SetActive(true);
        }
        
        private static void SetCondition(Transform container, AbilityCardContainer ability, BoardGameConfig boardGameConfig, CardsViewConfigSO viewCard)
        {
            switch (ability.Condition)
            {
                case AbilityCondition.Corporates:
                    SetDoubleNations(container, boardGameConfig, viewCard, "Corporates");
                    break;
                case AbilityCondition.Guns:
                    SetDoubleNations(container, boardGameConfig, viewCard, "Guns");
                    break;
                case AbilityCondition.Nomads:
                    SetDoubleNations(container, boardGameConfig, viewCard, "Nomads");
                    break;
                case AbilityCondition.Netrunners:
                    SetDoubleNations(container, boardGameConfig, viewCard, "Netrunners");
                    break;
                case AbilityCondition.Destroy:
                    var textDestroy = Object.Instantiate(viewCard.TextBaseAbility, container);
                    //TODO поправить на локализацию
                    textDestroy.SetText("Destroy Card");
                    break;
            }

            var imageArrow = Object.Instantiate(viewCard.IconsArrowConditionAbility, container);
        }

        private static void SetDoubleNations(Transform container,  BoardGameConfig boardGameConfig, CardsViewConfigSO viewCard, string nationsName)
        {
            boardGameConfig.NationsImage.TryGetValue(nationsName, out var icons);
            var im_1 = Object.Instantiate(viewCard.IconsBaseAbility, container);
            im_1.sprite = icons;
        }

        private static void SetAction(Transform container, AbilityCardContainer ability, BoardGameConfig boardGameConfig, CardsViewConfigSO viewCard, CardsConfig cardConfig, bool oneAbility = false)
        {
            if (ability.AbilityType == AbilityType.Trade)
            {
                boardGameConfig.CurrencyImage.TryGetValue("Trade", out var imTrade);
                AddBaseAction(container, imTrade, ability.Count, boardGameConfig.ColorTradeText, viewCard, oneAbility);
            }
            else
            {
                cardConfig.AbilityCard.TryGetValue(ability.AbilityType.ToString(), out var abilityCardConfig);
                var textCard = Object.Instantiate(viewCard.TextBaseAbility, container);

                if (ability.Count > 1 && abilityCardConfig.AbilityLocMultiple != null)
                {
                    textCard.SetParameters(ability.Count);
                    textCard.SetText(abilityCardConfig.AbilityLocMultiple);
                }
                else
                    textCard.SetText(abilityCardConfig.AbilityLoc);
            }
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