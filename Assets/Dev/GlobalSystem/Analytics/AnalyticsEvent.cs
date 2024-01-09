using System;

namespace CyberNet.Global.Analytics
{
    public static class AnalyticsEvent
    {
        public static Action<string> StartProgressEvent;
        public static Action<string> CompleteProgressEvent;
        public static Action<string> FailProgressEvent;
        
        
        public static Action<int> SessionTime;
    }
}