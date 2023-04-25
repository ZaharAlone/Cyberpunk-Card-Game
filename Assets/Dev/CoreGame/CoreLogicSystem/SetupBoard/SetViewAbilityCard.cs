using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BoardGame.Core
{
    public static class SetViewAbilityCard
    {
        public static void SetView(Transform container, AbilityCard ability, BoardGameConfig boardGameConfig)
        {
            if (ability.Condition != AbilityCondition.None)
                SetConfition(container, ability, boardGameConfig);

            SetAction(container, ability, boardGameConfig);
            Object.Instantiate(boardGameConfig.TextDotAbility, container);

            container.gameObject.SetActive(true);
        }

        private static void SetConfition(Transform container, AbilityCard ability, BoardGameConfig boardGameConfig)
        {
            switch (ability.Condition)
            {
                case AbilityCondition.cyberpsychosis_5:
                    var textC5 = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textC5.text = "cyberpsychosis > 5";
                    break;
                case AbilityCondition.cyberpsychosis_10:
                    var textC10 = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textC10.text = "cyberpsychosis > 10";
                    break;
                case AbilityCondition.cyberpsychosis_15:
                    var textC15 = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textC15.text = "cyberpsychosis > 15";
                    break;
                case AbilityCondition.doubleCorporates:
                    var imCorpo = Object.Instantiate(boardGameConfig.IconsBaseAbility, container);
                    break;
                case AbilityCondition.doubleGuns:
                    var imGuns = Object.Instantiate(boardGameConfig.IconsBaseAbility, container);
                    break;
                case AbilityCondition.doubleNomads:
                    var imNomads = Object.Instantiate(boardGameConfig.IconsBaseAbility, container);
                    break;
                case AbilityCondition.doubleNetrunners:
                    var imNetrunners = Object.Instantiate(boardGameConfig.IconsBaseAbility, container);
                    break;
                case AbilityCondition.destroyCard:
                    var textDestroy = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDestroy.text = "Destroy Card";
                    break;
            }

            var imageArrow = Object.Instantiate(boardGameConfig.IconsMiniAbility, container);
        }

        private static void SetAction(Transform container, AbilityCard ability, BoardGameConfig boardGameConfig)
        {
            switch (ability.Action)
            {
                case AbilityAction.attack:
                    boardGameConfig.CurrencyImage.TryGetValue("Attack", out var imAttack);
                    AddBaseAction(container, imAttack, ability.Count, boardGameConfig);
                    break;
                case AbilityAction.trade:
                    boardGameConfig.CurrencyImage.TryGetValue("Trade", out var imTrade);
                    AddBaseAction(container, imTrade, ability.Count, boardGameConfig);
                    break;
                case AbilityAction.influence:
                    boardGameConfig.CurrencyImage.TryGetValue("Influence", out var imInfluence);
                    AddBaseAction(container, imInfluence, ability.Count, boardGameConfig);
                    break;
                case AbilityAction.drawCard:
                    var textDraw = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDraw.text = "Draw Card";
                    break;
                case AbilityAction.discardCard:
                    var textDiscard = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDiscard.text = "Discard Card Enemy";
                    break;
                case AbilityAction.destroyCard:
                    var textDestroy = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDestroy.text = "Destroy Card";
                    break;
                case AbilityAction.up—yberpsychosis:
                    var textUpCyb = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textUpCyb.text = "Up Cyberpsychosis";
                    break;
                case AbilityAction.downCyberpsychosis:
                    var textDownCyb = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textDownCyb.text = "Down Cyberpsychosis";
                    break;
                case AbilityAction.cloneCard:
                    var textClone = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textClone.text = "Clone card in hand";
                    break;
                case AbilityAction.noiseCard:
                    var textNoise = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textNoise.text = "Add Card Noise Enemy";
                    break;
                case AbilityAction.thiefCard:
                    var textThief = Object.Instantiate(boardGameConfig.TextBaseAbility, container);
                    textThief.text = "Thief Random Card";
                    break;
            }
        }

        private static void AddBaseAction(Transform container, Sprite sprite, int count, BoardGameConfig boardGameConfig)
        {
            var imageAdd = Object.Instantiate(boardGameConfig.IconsMiniAbility, container);

            for (int i = 0; i < count; i++)
            {
                var image = Object.Instantiate(boardGameConfig.IconsBaseAbility, container);
                image.sprite = sprite;
            }
        }
    }
}