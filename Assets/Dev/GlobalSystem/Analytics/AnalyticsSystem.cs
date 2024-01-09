using System;
using GameAnalyticsSDK;
using UnityEngine;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Global.Analytics
{
    [EcsSystem(typeof(GlobalModule))]
    public class AnalyticsSystem : IPreInitSystem
    {
        public void PreInit()
        {
            AnalyticsEvent.StartProgressEvent += StartProgressEvent;
            AnalyticsEvent.CompleteProgressEvent += CompleteProgressEvent;
            AnalyticsEvent.FailProgressEvent += FailProgressEvent;
            AnalyticsEvent.SessionTime += EventSessionTime;
        }
        
        private void StartProgressEvent(string keyEvent)
        {
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Start, keyEvent);
        }
        
        private void CompleteProgressEvent(string keyEvent)
        {
            GameAnalytics.NewProgressionEvent (GAProgressionStatus.Complete, keyEvent);
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