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
        }
    }
}