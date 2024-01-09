using System;

namespace CyberNet.Meta.StartGame
{
    public static class StartGameAction
    {
        public static Action StartLocalGame;
        public static Action StartCampaign;
        public static Action StartTutorial;
        public static Action<string> StartServer;
    }
}