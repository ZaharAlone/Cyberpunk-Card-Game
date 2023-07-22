using System;

namespace CyberNet.Meta
{
    public static class StartGameAction
    {
        public static Action<string> StartGameLocalVSAI;
        public static Action StartGameLocalVSPlayer;
        public static Action StartCampaign;
    }
}