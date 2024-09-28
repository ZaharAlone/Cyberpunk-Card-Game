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
            SetupCardAction.CreateCard += CreateCard;
            SetupCardAction.SetViewCardNotInitToDeck += SetViewCardNotInit;
        }

        private Entity CreateCard(CardData placeCard, Transform parent, bool isPlayerCard)
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
            var cardsViewConfig = boardGameData.CardsViewConfig;
            
            cardsViewConfig.CardsImageDictionary.TryGetValue(cardConfigJson.ImageKey, out var cardImage);

            if (cardImage == null)
            {
                Debug.LogError($"Not find card image is name: {cardConfigJson.ImageKey}");
                return;
            }

            cardFace.SetViewCard(cardImage, cardConfigJson.Header, cardConfigJson.ValueLeftPoint, cardConfigJson.ValueRightPoint, cardConfigJson.Price);

            if (cardConfigJson.CardType == CardType.TradeRow)
            {
                for (int i = 0; i < cardConfigJson.Count; i++)
                    Object.Instantiate(cardsViewConfig.ItemIconsCounterCard, cardFace.CountCardBlock);   
            }
            
            var isAbility_0 = cardConfigJson.Ability_0.AbilityType != AbilityType.None;
            var isAbility_1 = cardConfigJson.Ability_1.AbilityType != AbilityType.None;
            
            if (!isAbility_0 && !isAbility_1 && cardConfigJson.ValueLeftPoint == 0 && cardConfigJson.ValueRightPoint == 0)
                cardFace.HideDownBlock();
            
            var isOneAbility = isAbility_0 && !isAbility_1;
            
            var abilityIsDifferentType = CheckAbilityIsDifferentType(cardConfigJson.Ability_0.AbilityType, cardConfigJson.Ability_1.AbilityType);

            var isShowChooseFromTwoAbilities = isAbility_0 && isAbility_1 && abilityIsDifferentType;
            
            cardFace.SetHeaderSelectAbility(isShowChooseFromTwoAbilities);
            
            if (isOneAbility)
                cardFace.AbilityBlock_1_Container.gameObject.SetActive(false);
            else
                cardFace.SetBigDownBlock();
            
            if (cardConfigJson.Ability_0.AbilityType != AbilityType.None)
            {
                if (isOneAbility)
                    SetViewAbilityCard.SetView(cardFace.AbilityBlock_OneShot_Container, cardConfigJson.Ability_0, boardGameData, cardConfig, isOneAbility, abilityIsDifferentType);
                else
                    SetViewAbilityCard.SetView(cardFace.AbilityBlock_0_Container, cardConfigJson.Ability_0, boardGameData, cardConfig, isOneAbility, abilityIsDifferentType);
            }
            
            if (isAbility_1)
                SetViewAbilityCard.SetView(cardFace.AbilityBlock_1_Container, cardConfigJson.Ability_1, boardGameData, cardConfig, isOneAbility, abilityIsDifferentType);

            if (isBackCard)
                card.CardOnBack();
        }

        //Проверяем, можем ли разыграть обе абилки на карте или нет
        private bool CheckAbilityIsDifferentType(AbilityType abilityType_0, AbilityType abilityType_1)
        {
            var abilityPlayinOnMap_0 = AbilityCardUtilsAction.CheckAbilityIsPlayingOnMap.Invoke(abilityType_0);
            var abilityPlayinOnMap_1 = AbilityCardUtilsAction.CheckAbilityIsPlayingOnMap.Invoke(abilityType_1);
            
            if (abilityPlayinOnMap_0 && abilityPlayinOnMap_1)
                return true;
            else
                return false;
        }

        public void Destroy()
        {
            SetupCardAction.CreateCard -= CreateCard;
            SetupCardAction.SetViewCardNotInitToDeck -= SetViewCardNotInit;
        }
    }
}