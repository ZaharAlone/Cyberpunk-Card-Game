using System;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Meta
{
    public static class SelectLeaderAction
    {
        public static Action<SelectLeadersData> OpenSelectLeaderUI;
        public static Action CloseSelectLeaderUI;
        public static Action<string> SelectLeader;
        public static Action BackMainMenu;
        public static Action ConfirmSelect;

        public static Func<string, bool, Sprite> InitButtonLeader;
    }
}