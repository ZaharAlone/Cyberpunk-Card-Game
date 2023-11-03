using System.Threading.Tasks;
using CyberNet.Core.Traderow;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.UI.CardPopup
{
    [EcsSystem(typeof(CoreModule))]
    public class CardPopupSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private bool _closePopup;
        
        public void PreInit()
        {
            CardPopupAction.OpenPopupCard += OpenPopupCard;
            CardPopupAction.ClosePopupCard += ClosePopupCard;
        }
        
        private async void OpenPopupCard(string guidCard, bool traderow)
        {
            var cardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();

            var cardComponent = cardEntity.GetComponent<CardComponent>();
            var popupConfig = _dataWorld.OneData<CardsConfig>().PopupCard;
            popupConfig.TryGetValue(cardComponent.Key, out var popupCardConfig);
            
            if (!popupCardConfig.IsPoput)
                return;

            var uiPopup = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardPopupMono;
            var gameConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            
            var waitTime = 100;
            var scale = gameConfig.SizeSelectCardHand.x;
            
            if (traderow)
            {
                if (_dataWorld.Select<TraderowIsShowComponent>().Count() == 0)
                {
                    waitTime = 370;
                }
                
                scale = gameConfig.SizeSelectCardTradeRow.x;
            }
            _closePopup = false;
            
            await Task.Delay(waitTime);
            
            if (_closePopup)
                return;
            
            uiPopup.SetView(popupCardConfig.AbilityDescrLoc, popupCardConfig.ArtisticDescrLoc);
            uiPopup.Positioning(cardComponent.RectTransform, scale);
        }

        private void ClosePopupCard()
        {
            _closePopup = true;
            var uiPopup = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardPopupMono;
            uiPopup.ClosePopup();
        }
        
        public void Destroy()
        {
            CardPopupAction.OpenPopupCard -= OpenPopupCard;
            CardPopupAction.ClosePopupCard -= ClosePopupCard;
        }
    }
}