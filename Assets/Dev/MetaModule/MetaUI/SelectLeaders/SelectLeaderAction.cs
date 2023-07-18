using System;
using CyberNet.Global;

namespace CyberNet.Meta
{
    public static class SelectLeaderAction
    {
        public static Action<GameModeEnum> OpenSelectLeaderUI;
        public static Action<string> SelectLeader;
        public static Action BackMainMenu;
        public static Action StartGame;
    }
}