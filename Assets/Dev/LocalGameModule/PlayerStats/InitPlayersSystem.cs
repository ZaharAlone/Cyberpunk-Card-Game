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
            ref var startGameConfig = ref _dataWorld.OneData<StartGameConfig>();
            
            CreatePlayer(startGameConfig.Player_1);
            CreatePlayer(startGameConfig.Player_2);
            CreatePlayer(startGameConfig.Player_3);
            CreatePlayer(startGameConfig.Player_4);
        }
        
        private void CreatePlayer(PlayerConfigStartGame playerConfig)
        {
            if (playerConfig.PlayerType == PlayerType.None)
                return;
            
            ref var config = ref _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var entity = _dataWorld.NewEntity();

            entity.AddComponent(new PlayerStatsComponent {
                PlayerType = playerConfig.PlayerType,
                PlayerID = 1, 
                UnitCount = config.StartCountUnit, 
                VictoryPoint = 0
            });

            entity.AddComponent(new PlayerViewComponent {
                Key = playerConfig.Key, 
                AvatarKey = playerConfig.AvatarKey, 
                Name = playerConfig.Name
            });
        }
    }
}