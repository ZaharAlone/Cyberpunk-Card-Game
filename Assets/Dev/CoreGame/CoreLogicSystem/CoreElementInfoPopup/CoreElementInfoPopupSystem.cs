using System.Threading.Tasks;
using CyberNet.Core.Traderow;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CyberNet.Core.UI.CorePopup
{
    [EcsSystem(typeof(CoreModule))]
    public class CoreElementInfoPopupSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private bool _closePopup;
        
        public void PreInit()
        {
            CoreElementInfoPopupAction.OpenPopupCard += OpenPopupCard;
            CoreElementInfoPopupAction.OpenPopupButton += OpenPopupButton;
            CoreElementInfoPopupAction.ClosePopupCard += ClosePopupCard;
        }

        private async void OpenPopupCard(string guidCard, bool traderow)
        {
            var cardEntity = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();

            var cardComponent = cardEntity.GetComponent<CardComponent>();
            var popupConfig = _dataWorld.OneData<CorePopupData>().PopupConfig;
            popupConfig.TryGetValue(cardComponent.Key, out var popupCardConfig);
            
            if (!popupCardConfig.IsPoput)
                return;

            var uiPopup = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.GameElementInfoPopupMono;
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

            uiPopup.SetView(popupCardConfig.DescrLoc, popupCardConfig.HeaderLoc, popupCardConfig.ArtisticDescrLoc);
            uiPopup.PositioningLeftRight(cardComponent.RectTransform, 10, scale);
        }

        private void OpenPopupButton(RectTransform rectTransform, string keyPopup)
        {
            var popupConfig = _dataWorld.OneData<CorePopupData>().PopupConfig;
            popupConfig.TryGetValue(keyPopup, out var popupViewConfig);
            
            if (!popupViewConfig.IsPoput)
                return;

            var uiPopup = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.GameElementInfoPopupMono;
            
            uiPopup.SetView(popupViewConfig.DescrLoc, popupViewConfig.HeaderLoc, popupViewConfig.ArtisticDescrLoc);
            uiPopup.PositioningUp(rectTransform, 10);
            Debug.LogError("Show up popup");
        }
        
        private void ClosePopupCard()
        {
            _closePopup = true;
            var uiPopup = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.GameElementInfoPopupMono;
            uiPopup.ClosePopup();
        }

        public void Destroy()
        {
            CoreElementInfoPopupAction.OpenPopupCard -= OpenPopupCard;
            CoreElementInfoPopupAction.OpenPopupButton -= OpenPopupButton;
            CoreElementInfoPopupAction.ClosePopupCard -= ClosePopupCard;
        }
    }
}