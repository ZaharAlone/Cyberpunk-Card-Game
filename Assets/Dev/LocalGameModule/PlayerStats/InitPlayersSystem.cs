using CyberNet.Global;
using CyberNet.Meta;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core
{
    [EcsSystem(typeof(LocalGameModule))]
    public class InitPlayersSystem : IActivateSystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            ref var selectPlayer = ref _dataWorld.OneData<SelectPlayerData>();

            for (int i = 0; i < selectPlayer.SelectLeaders.Count; i++)
            {
                CreatePlayer(selectPlayer.SelectLeaders[i]);
            }
        }
        
        private void CreatePlayer(SelectLeadersData selectLeadersData)
        {
            if (selectLeadersData.PlayerType == PlayerType.None)
                return;
            
            ref var config = ref _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var entity = _dataWorld.NewEntity();

            entity.AddComponent(new PlayerStatsComponent {
                PlayerType = selectLeadersData.PlayerType,
                PlayerID = 1, 
                UnitCount = config.StartCountUnit, 
                VictoryPoint = 0
            });

            entity.AddComponent(new PlayerViewComponent {
                LeaderKey = selectLeadersData.SelectLeader, 
                Name = selectLeadersData.NamePlayer
            });
        }
    }
}