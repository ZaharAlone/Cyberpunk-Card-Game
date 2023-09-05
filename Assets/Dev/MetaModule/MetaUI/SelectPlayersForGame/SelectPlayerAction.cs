using System;
namespace CyberNet.Meta.SelectPlayersForGame
{
    public static class SelectPlayerAction
    {
        public static Action OpenSelectPlayerUI;
        public static Action CloseSelectPlayerUI;
        public static Action OnClickBack;
        public static Action<int> OnClickEditLeader;

        public static Action<int> ClearSlot;
        public static Action<int> CreatePlayer;
        
        public static Action<int, string> SetPlayerName;
    }
}