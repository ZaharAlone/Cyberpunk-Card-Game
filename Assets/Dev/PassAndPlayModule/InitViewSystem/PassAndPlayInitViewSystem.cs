using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core
{
    [EcsSystem(typeof(PassAndPlayModule))]
    public class PassAndPlayInitViewSystem : IActivateSystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            _dataWorld.CreateOneData(new ViewPlayerData { PlayerView = PlayerEnum.Player1 });
            _dataWorld.CreateOneData(new Player2ViewData { Name = "Player2", AvatarKey = "avatar_red_witch" });
        }
    }
}