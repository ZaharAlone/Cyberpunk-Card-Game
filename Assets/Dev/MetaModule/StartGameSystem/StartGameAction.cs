using System;

namespace CyberNet.Meta.StartGame
{
    public static class StartGameAction
    {
        public static Action<string> StartGameLocalVSAI;
        public static Action<string, string> StartGameLocalVSPlayer;
        public static Action StartCampaign;
        public static Action<string> StartServer;
    }
}