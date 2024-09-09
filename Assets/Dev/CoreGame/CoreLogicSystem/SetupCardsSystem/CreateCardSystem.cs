using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.AbilityCard;
using Object = UnityEngine.Object;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class CreateCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SetupCardAction.InitCard += InitCard;
            SetupCardAction.SetViewCardNotInit += SetViewCardNotInit;
        }

        private Entity InitCard(CardData placeCard, Transform parent, bool isPlayerCard)
        {
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            var boardGameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var cardMono = Object.Instantiate(boardGameConfig.CardGO, parent);
            var cardGO = cardMono.gameObject;
            cardsConfig.Cards.TryGetValue(placeCard.CardName, out var card);

            SetViewCard(cardMono, card, true);

            var entity = _dataWorld.NewEntity();
            var cardComponent = SetCardComponent.Set(cardGO, card, cardMono);
            
            if (isPlayerCard)
            {
                cardGO.transform.localScale = boardGameConfig.SizeCardInDeck;
            }
            else // card is trade row
            {
                cardComponent.PlayerID = -1;
                cardGO.transform.localScale = boardGameConfig.SizeCardInTraderow;
            }
            
            entity.AddComponent(cardComponent);
            entity.AddComponent(new CardSortingIndexComponent { Index = placeCard.IDPositions });
            return entity;
        }

        private void SetViewCardNotInit(CardMono cardMono, string keyCard)
        {
            var cardsConfig = _dataWorld.OneData<CardsConfig>();
            cardsConfig.Cards.TryGetValue(keyCard, out var cardConfigJson);
            
            SetViewCard(cardMono, cardConfigJson, false);
        }
        
        private void SetViewCard(CardMono card, CardConfigJson cardConfigJson, bool isBackCard)
        {
            var cardFace = card.CardFaceMono;
            var cardConfig = _dataWorld.OneData<CardsConfig>();
            
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var boardGameConfig = boardGameData.BoardGameConfig;
            var cardsViewConfig = boardGameData.CardsViewConfig;
            
            cardsViewConfig.CardsImageDictionary.TryGetValue(cardConfigJson.ImageKey, out var cardImage);

            if (cardImage == null)
            {
                Debug.LogError($"Not find card image is name: {cardConfigJson.ImageKey}");
                return;
            }

            if (cardConfigJson.Nations != "Neutral")
            {
                boardGameConfig.NationsImage.TryGetValue(cardConfigJson.Nations, out var nationsImage);
                cardFace.SetViewCard(cardImage, cardConfigJson.Header, cardConfigJson.CyberpsychosisCount, cardConfigJson.Price, nationsImage);

                for (int i = 0; i < cardConfigJson.Count; i++)
                    Object.Instantiate(cardsViewConfig.ItemIconsCounterCard, cardFace.CountCardBlock);
            }
            else
                cardFace.SetViewCard(cardImage, cardConfigJson.Header, cardConfigJson.CyberpsychosisCount, cardConfigJson.Price);

            var isAbility_0 = cardConfigJson.Ability_0.AbilityType != AbilityType.None;
            var isAbility_1 = cardConfigJson.Ability_1.AbilityType != AbilityType.None;
            var isAbility_2 = cardConfigJson.Ability_2.AbilityType != AbilityType.None;
            
            var onlyOneAbility = isAbility_0 && !isAbility_1 && !isAbility_2;
            var isSelectTwoAbility = isAbility_0 && isAbility_1;
            
            var abilityIsDifferentType = CheckAbilityIsDifferentType(cardConfigJson.Ability_0.AbilityType, cardConfigJson.Ability_1.AbilityType);
            
            cardFace.SetHeaderSelectAbility(isSelectTwoAbility, abilityIsDifferentType);
            cardFace.IsConditionAbility(isAbility_2);
            
            if (!isSelectTwoAbility)
                cardFace.AbilityBlock_1_Container.gameObject.SetActive(false);
            
            if (isSelectTwoAbility && isAbility_2)
                cardFace.SetBigDownBlock();
            
            if (cardConfigJson.Ability_0.AbilityType != AbilityType.None)
            {
                if (onlyOneAbility)
                    SetViewAbilityCard.SetView(cardFace.AbilityBlock_OneShot_Container, cardConfigJson.Ability_0, boardGameData, cardConfig, isSelectTwoAbility, abilityIsDifferentType, onlyOneAbility);
                else
                    SetViewAbilityCard.SetView(cardFace.AbilityBlock_0_Container, cardConfigJson.Ability_0, boardGameData, cardConfig, isSelectTwoAbility, abilityIsDifferentType);
            }
            
            if (cardConfigJson.Ability_1.AbilityType != AbilityType.None)
                SetViewAbilityCard.SetView(cardFace.AbilityBlock_1_Container, cardConfigJson.Ability_1, boardGameData, cardConfig, isSelectTwoAbility, abilityIsDifferentType);
            if (cardConfigJson.Ability_2.AbilityType != AbilityType.None)
                SetViewAbilityCard.SetView(cardFace.AbilityBlock_2_Container, cardConfigJson.Ability_2, boardGameData, cardConfig);

            if (isBackCard)
                card.CardOnBack();
        }

        private bool CheckAbilityIsDifferentType(AbilityType abilityType_0, AbilityType abilityType_1)
        {
            var abilityOnlyMap_0 = AbilityCardUtilsAction.CheckAbilityIsPlayingOnlyMap.Invoke(abilityType_0);
            var abilityOnlyMap_1 = AbilityCardUtilsAction.CheckAbilityIsPlayingOnlyMap.Invoke(abilityType_1);
            
            var abilityOnlyArena_0 = AbilityCardUtilsAction.CheckAbilityIsPlayingOnlyArena.Invoke(abilityType_0);
            var abilityOnlyArena_1 = AbilityCardUtilsAction.CheckAbilityIsPlayingOnlyArena.Invoke(abilityType_1);
            
            if (abilityOnlyMap_0 && abilityOnlyArena_1 || abilityOnlyArena_0 && abilityOnlyMap_1)
                return true;
            else
                return false;
        }

        public void Destroy()
        {
            SetupCardAction.InitCard -= InitCard;
            SetupCardAction.SetViewCardNotInit -= SetViewCardNotInit;
        }
    }
}