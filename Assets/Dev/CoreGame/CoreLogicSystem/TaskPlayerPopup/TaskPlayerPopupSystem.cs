using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI.CorePopup;

namespace CyberNet.Core.UI.TaskPlayerPopup
{
    [EcsSystem(typeof(CoreModule))]
    public class TaskPlayerPopupSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            TaskPlayerPopupAction.OpenPopupSelectFirstBase += OpenPopupSelectFirstBase;
            TaskPlayerPopupAction.ClosePopup += ClosePopup;
            TaskPlayerPopupAction.OpenPopup += OpenPopup;
            TaskPlayerPopupAction.OpenPopupParam += OpenPopupParam;
        }
        
        private void OpenPopupSelectFirstBase()
        {
            _dataWorld.OneData<CorePopupData>().CorePopupTaskConfig.TryGetValue("popupTask_selectStartAreal", out var configPopup);
            OpenPopup(configPopup.HeaderLoc, configPopup.DescrLoc);
        }
        
        private void OpenPopup(string header, string descr)
        {
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.TaskPlayerPopupUIMono.OpenWindowSetLocalizeTerm(header, descr);
            boardGameUI.TraderowMono.FullHideTradeRowAnimations();
        }

        private void OpenPopupParam(string header, string descr, string param)
        {
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.TaskPlayerPopupUIMono.OpenWindowSetLocalizeTerm(header, descr);
            boardGameUI.TaskPlayerPopupUIMono.SerLocalizeParameter(param);
            boardGameUI.TraderowMono.FullHideTradeRowAnimations();
        }
        
        private void ClosePopup()
        {
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            boardGameUI.TaskPlayerPopupUIMono.CloseWindow();

            var isCardForBuy = _dataWorld.Select<CardFreeToBuyComponent>().Count() > 0;
            
            if (isCardForBuy)
                boardGameUI.TraderowMono.ShowFullTradeRowPanelAnimations();
            else
                boardGameUI.TraderowMono.TradeRowToMiniPanelAnimations();
        }

        public void Destroy()
        {
            TaskPlayerPopupAction.OpenPopupSelectFirstBase -= OpenPopupSelectFirstBase;
            TaskPlayerPopupAction.ClosePopup -= ClosePopup;
            TaskPlayerPopupAction.OpenPopup -= OpenPopup;
            TaskPlayerPopupAction.OpenPopupParam -= OpenPopupParam;
        }
    }
}