using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace BoardGame.Core
{
    [EcsSystem(typeof(VSAIModule))]
    public class InitGameVSAISystem : IActivateSystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            _dataWorld.CreateOneData(new ViewPlayerData { PlayerView = PlayerEnum.Player1 });
            _dataWorld.CreateOneData(new Player2ViewData { Name = "Bot", AvatarKey = "avatar_red_witch" });
        }
    }
}