using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Tutorial.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class TutorialUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            TutorialUIAction.OpenPopupZoomCamera += OpenPopupZoomCamera;
            TutorialUIAction.OpenPopupMoveCamera += OpenPopupMoveCamera;
            TutorialUIAction.ClosePopup += ClosePopup;
        }
        
        private void OpenPopupZoomCamera()
        {
            var tutorialPopup = _dataWorld.OneData<TutorialData>().TutorialUIMono.TutorialPopupUIMono;
            tutorialPopup.OpenZoomCamera(false);
        }
        
        private void OpenPopupMoveCamera()
        {
            var tutorialPopup = _dataWorld.OneData<TutorialData>().TutorialUIMono.TutorialPopupUIMono;
            tutorialPopup.OpenMoveCamera(false);    
        }
        
        private void ClosePopup()
        {
            var tutorialPopup = _dataWorld.OneData<TutorialData>().TutorialUIMono.TutorialPopupUIMono;
            tutorialPopup.CloseAllPopup();
        }

        public void Destroy()
        {
            TutorialUIAction.OpenPopupZoomCamera -= OpenPopupZoomCamera;
            TutorialUIAction.OpenPopupMoveCamera -= OpenPopupMoveCamera;
            TutorialUIAction.ClosePopup -= ClosePopup;
        }
    }
}