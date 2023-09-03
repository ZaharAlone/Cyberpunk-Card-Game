using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.SelectFirstBase
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectFirstBaseSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            
        }
    }
}