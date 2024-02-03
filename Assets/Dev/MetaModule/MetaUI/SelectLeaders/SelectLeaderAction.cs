using System;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Meta
{
    public static class SelectLeaderAction
    {
        public static Action<SelectLeaderData, bool> OpenSelectLeaderUI;
        public static Action<string> SelectLeader;
        public static Action BackMainMenu;
        public static Action ConfirmSelect;

        public static Func<string, bool, Sprite> InitButtonLeader;
    }
}