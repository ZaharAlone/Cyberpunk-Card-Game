using System.Collections.Generic;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Core.AbilityCard.DiscardCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CyberNet.Tools.DebugGame
{
    [EcsSystem(typeof(DebugModule))]
    public class DebugSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            DebugAction.GetCard += GetCard;
            DebugAction.ReadyConfigCards += ReadyInitDebug;
            DebugAction.AddDiscardCard += AddDiscardCard;
        }
        
        public void ReadyInitDebug()
        {
            LoadingUI();
        }

        private async Task LoadingUI()
        {
            var taskUIDebug = Addressables.LoadAssetAsync<GameObject>("DebugCanvas").Task;
            await taskUIDebug;
            
            var uiMono = Object.Instantiate(taskUIDebug.Result).GetComponent<DebugUIMono>();
            var debugData = new DebugData();
            debugData.DebugUIMono = uiMono;
            
            _dataWorld.CreateOneData(debugData);
            SetListCardConfig();
        }

        public void SetListCardConfig()
        {
            var cardConfig = _dataWorld.OneData<CardsConfig>().Cards;
            var keysList = new List<string>(cardConfig.Keys);

            var debugUI = _dataWorld.OneData<DebugData>().DebugUIMono;
            debugUI.SetListCardGame(keysList);
        }
        
        private void GetCard(string cardName)
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var countCardPlayer = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .Count();
            
            var cardsParent = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            var cardData = new CardData {
                IDPositions = countCardPlayer, CardName = cardName
            };
            var entity = SetupCardAction.InitCard.Invoke(cardData, cardsParent, true);
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            cardComponent.PlayerID = currentPlayerID;
            entity.AddComponent(new CardPlayerComponent());
            
            entity.AddComponent(new CardHandComponent());
            entity.AddComponent(new WaitAnimationsDrawHandCardComponent { PlayerID = cardComponent.PlayerID, WaitTime = 0f});
            cardComponent.CardMono.ShowCard();
            
            WaitEndGetCard();
        }

        private async void WaitEndGetCard()
        {
            await Task.Delay(300);
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
        }

        private void AddDiscardCard()
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            if (currentPlayerEntity.HasComponent<PlayerEffectDiscardCardComponent>())
            {
                ref var discardCardComponent = ref currentPlayerEntity.GetComponent<PlayerEffectDiscardCardComponent>();
                discardCardComponent.Count++;
            }
            else
            {
                var discardCardComponent = new PlayerEffectDiscardCardComponent {
                    Count = 1
                };
                currentPlayerEntity.AddComponent(discardCardComponent);
            }
        }

        public void Destroy()
        {
            DebugAction.GetCard -= GetCard;
            DebugAction.ReadyConfigCards -= ReadyInitDebug;
        }
    }
}