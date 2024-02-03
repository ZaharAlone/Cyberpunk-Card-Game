using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using Steamworks;

namespace CyberNet.Platform
{
    [EcsSystem(typeof(GlobalModule))]
    public class SetPlayerName : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            PlatformAction.GetPlayerName += GetPlayerName;
        }

        private string GetPlayerName()
        {
            var playerName = "Player";
            
            #if STEAM
            if(SteamManager.Initialized) {
                playerName = SteamFriends.GetPersonaName();
            }
            #endif

            return playerName;
        }

        public void Destroy()
        {
            PlatformAction.GetPlayerName -= GetPlayerName;
        }
    }
}