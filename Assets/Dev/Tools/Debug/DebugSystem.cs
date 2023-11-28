using System.Collections.Generic;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Core.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CyberNet.Tools.DebugGame
{
    [EcsSystem(typeof(DebugModule))]
    public class DebugSystem : IPreInitSystem, IInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            DebugAction.GetCard += GetCard;
        }
        
        public void Init()
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
            var waitTimeAnim = SortingDeckCardAnimationsAction.GetTimeCardToHand.Invoke(cardComponent.PlayerID);
            waitTimeAnim += 0.175f;
            entity.AddComponent(new WaitAnimationsDrawHandCardComponent { PlayerID = cardComponent.PlayerID, WaitTime = waitTimeAnim });
            cardComponent.CardMono.ShowCard();
            
            WaitEndGetCard(waitTimeAnim);
        }

        private async void WaitEndGetCard(float time)
        {
            await Task.Delay(450);
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
        }
    }
}