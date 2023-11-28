using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaSystem : IInitSystem, IRunSystem, IDeactivateSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            
        }

        public void Run()
        {
            
        }

        public void Deactivate()
        {
            
        }
    }
}