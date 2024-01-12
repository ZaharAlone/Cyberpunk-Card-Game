using System;
using GameAnalyticsSDK;
using UnityEngine;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using Object = UnityEngine.Object;

#if !UNITY_EDITOR
namespace CyberNet.Global.Analytics
{
    [EcsSystem(typeof(GlobalModule))]
    public class AnalyticsSystem : IPreInitSystem, IInitSystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            AnalyticsEvent.StartProgressEvent += StartProgressEvent;
            AnalyticsEvent.CompleteProgressEvent += CompleteProgressEvent;
            AnalyticsEvent.CompleteTwoProgressEvent += CompleteTwoProgressEvent;
            AnalyticsEvent.FailProgressEvent += FailProgressEvent;
            AnalyticsEvent.SessionTime += EventSessionTime;
            
            var analyticsGO = _dataWorld.OneData<BoardGameData>().BoardGameConfig.AnalyticsGO;
            Object.Instantiate(analyticsGO);
        }

        public void Init()
        {
            GameAnalytics.SetCustomId(SystemInfo.deviceUniqueIdentifier);
            GameAnalytics.Initialize();
        }

        private void StartProgressEvent(string keyEvent)
        {
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, keyEvent);
        }

        private void CompleteProgressEvent(string keyEvent)
        {
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, keyEvent);
        }
        
        private void CompleteTwoProgressEvent(string progression_1, string progression_2)
        {
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, progression_1, progression_2);

        }

        private void FailProgressEvent(string keyEvent)
        {
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Fail, keyEvent);
        }

        private void EventSessionTime(int time)
        {
            GameAnalytics.NewDesignEvent ("Session Time", time);
        }
    }
}
#endif