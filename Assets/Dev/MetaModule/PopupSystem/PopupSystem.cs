using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using UnityEngine;
using System;

namespace BoardGame.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class PopupSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            PopupAction.WaitPopup += OpenWaitPopup;
            PopupAction.WarningPopup += OpenWarningPopup;
            PopupAction.ConfirmPopup += OpenConfirmPopup;
            PopupAction.CloseWaitPopup += CloseWaitPopup;
            PopupAction.CloseWarningPopup += CloseWarningPopup;
            PopupAction.CloseConfirmPopup += CloseConfirmPopup;
        }

        private void OpenWaitPopup(string header)
        {
            var popup = _dataWorld.OneData<PopupData>().UIMono;
            popup.OpenWaitPopup(header);
        }

        private void OpenWarningPopup(string header, string descr, string buttonText, Action action)
        {
            var popup = _dataWorld.OneData<PopupData>().UIMono;
            popup.OpenWarningPopup(header, descr, buttonText, action);
        }

        private void OpenConfirmPopup(string header, string descr, string buttonRight, string buttonLeft, Action action)
        {
            var popup = _dataWorld.OneData<PopupData>().UIMono;
            popup.OpenConfirmPopup(header, descr, buttonRight, buttonLeft, action);
        }

        private void CloseWaitPopup()
        {
            var popup = _dataWorld.OneData<PopupData>().UIMono;
            popup.CloseWaitPopup();
        }

        private void CloseWarningPopup()
        {
            var popup = _dataWorld.OneData<PopupData>().UIMono;
            popup.CloseWarningPopup();
        }

        private void CloseConfirmPopup()
        {
            var popup = _dataWorld.OneData<PopupData>().UIMono;
            popup.CloseConfirmPopup();
        }
    }
}