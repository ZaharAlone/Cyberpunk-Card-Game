using System;

namespace CyberNet.Meta.StartGame
{
    public static class StartGameAction
    {
        public static Action<string> StartLocalGame;
        public static Action StartCampaign;
        public static Action<string> StartServer;
    }
}