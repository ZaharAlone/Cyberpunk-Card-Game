using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;
using CyberNet.SaveSystem;
using UnityEngine;

namespace CyberNet.Core.EndTurnWarningPopup
{
    [EcsSystem(typeof(CoreModule))]
    public class EndTurnWarningPopupSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            EndTurnWarningPopupAction.OpenPopupUnuseCard += OpenPopupUnuseCard;
            EndTurnWarningPopupAction.OpenPopupUnuseMoney += OpenPopupUnuseMoney;
            
            EndTurnWarningPopupAction.OnClickYes += ForceConfirmEndTurn;
            EndTurnWarningPopupAction.OnClickDontShow += OnClickDontShow;
        }
        
        private void OpenPopupUnuseCard()
        {
            var isOpenPopup = CheckIsOpenPopup();
            
            if (isOpenPopup)
            {
                var endTurnPopup = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.EndTurnWarningPopupUIMono;
                endTurnPopup.OpenPopupUnuseCard();
            }
            else
            {
                ForceConfirmEndTurn();
            }
        }
        
        private void OpenPopupUnuseMoney()
        {
            var isOpenPopup = CheckIsOpenPopup();
            
            if (isOpenPopup)
            {
                var endTurnPopup = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.EndTurnWarningPopupUIMono;
                endTurnPopup.OpenPopupUnuseMoney();
            }
            else
            {
                ForceConfirmEndTurn();
            }
        }
        
        private bool CheckIsOpenPopup()
        {
            var gameSettings = _dataWorld.OneData<SettingsData>().GameSettings;
            return gameSettings.IsShowWarningPopupEndTurn;
        }
        
        private void ForceConfirmEndTurn()
        {
            ActionPlayerButtonEvent.ForceEndRound?.Invoke();
        }
        
        private void OnClickDontShow()
        {
            ref var settingsData = ref _dataWorld.OneData<SettingsData>();
            settingsData.GameSettings.IsShowWarningPopupEndTurn = false;
            SaveAction.SaveSettingsGame?.Invoke();
            
            ForceConfirmEndTurn();
        }

        public void Destroy()
        {
            EndTurnWarningPopupAction.OpenPopupUnuseCard -= OpenPopupUnuseCard;
            EndTurnWarningPopupAction.OpenPopupUnuseMoney -= OpenPopupUnuseMoney;
            
            EndTurnWarningPopupAction.OnClickYes -= ForceConfirmEndTurn;
            EndTurnWarningPopupAction.OnClickDontShow -= OnClickDontShow;
        }
    }
}