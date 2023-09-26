using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using Steamworks;

namespace CyberNet.Platform.Steam
{
    [EcsSystem(typeof(SteamModule))]
    public class SteamSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            PlatformAction.GetPlayerName += GetPlayerName;
        }

        private string GetPlayerName()
        {
            #if UNITY_EDITOR
            return "Zakhar";
            #endif
            
            var playerName = "";
            if(SteamManager.Initialized) {
                playerName = SteamFriends.GetPersonaName();
            }
            else
            {
                playerName = "Player";
            }

            return playerName;
        }
    }
}