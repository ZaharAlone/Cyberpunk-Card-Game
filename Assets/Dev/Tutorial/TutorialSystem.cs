using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Dialog;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.Analytics;
using CyberNet.Tutorial;

namespace CyberNet.Tutorial
{
    [EcsSystem(typeof(TutorialGameModule))]
    public class TutorialSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            TutorialAction.StartFirstPlayerRound += StartTutorialPlayerRound;
        }

        private async void StartTutorialPlayerRound()
        {
            AnalyticsEvent.StartProgressEvent?.Invoke("start_tutorial");
            Debug.LogError("Start tutorial player Round");
            var uiRound = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ChangeRoundUI;
            var entityPlayer = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var playerViewComponent = entityPlayer.GetComponent<PlayerViewComponent>();
            uiRound.NewRoundView(playerViewComponent.Avatar, playerViewComponent.Name);
            await Task.Delay(1500);
            
            DialogAction.StartDialog?.Invoke("tutorial_start_intro");
            DialogAction.EndDialog += FinishIntroDialog;
        }
        
        private void FinishIntroDialog()
        {
            DialogAction.EndDialog -= FinishIntroDialog;
            AnalyticsEvent.CompleteProgressEvent?.Invoke("intro_dialog");
            
            AnalyticsEvent.FailProgressEvent?.Invoke("fail_tutorial");
            AnalyticsEvent.SessionTime?.Invoke(30);
        }

        public void Destroy()
        {
            
        }
    }
}