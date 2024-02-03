using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using UnityEngine;
using System;

namespace CyberNet.Meta
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
            var popup = _dataWorld.OneData<PopupData>().PopupUIMono;
            popup.OpenWaitPopup(header);
        }

        private void OpenWarningPopup(string header, string descr, string buttonText, Action action)
        {
            var popup = _dataWorld.OneData<PopupData>().PopupUIMono;
            popup.OpenWarningPopup(header, descr, buttonText, action);
        }

        private void OpenConfirmPopup(PopupConfimStruct popupData)
        {
            var popup = _dataWorld.OneData<PopupData>().PopupUIMono.PopupConfirmUIMono;
            popup.OpenPopup(popupData);
        }

        private void CloseWaitPopup()
        {
            var popup = _dataWorld.OneData<PopupData>().PopupUIMono;
            popup.CloseWaitPopup();
        }

        private void CloseWarningPopup()
        {
            var popup = _dataWorld.OneData<PopupData>().PopupUIMono;
            popup.CloseWarningPopup();
        }

        private void CloseConfirmPopup()
        {
            var popup = _dataWorld.OneData<PopupData>().PopupUIMono.PopupConfirmUIMono;
            popup.ClosePopupAnimation();
        }
    }
}