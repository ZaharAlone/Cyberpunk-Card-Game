using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace BoardGame.Core
{
    [EcsSystem(typeof(LocalGameModule))]
    public class InitPlayersSystem : IActivateSystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            ref var config = ref _dataWorld.OneData<BoardGameData>().BoardGameRule;
            _dataWorld.CreateOneData(new Player1StatsData { HP = config.BaseInfluenceCount, Cyberpsychosis = config.BaseCyberpsychosisCount });
            _dataWorld.CreateOneData(new Player2StatsData { HP = config.BaseInfluenceCount, Cyberpsychosis = config.BaseCyberpsychosisCount });
        }
    }
}