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
                CreatePlayer(selectPlayer.SelectLeaders[i], i);
            }
        }
        
        private void CreatePlayer(SelectLeaderData selectLeaderData, int positionInTurnQueue)
        {
            if (selectLeaderData.playerTypeEnum == PlayerTypeEnum.None)
                return;
            
            ref var config = ref _dataWorld.OneData<BoardGameData>().BoardGameRule;
            var leadersView = _dataWorld.OneData<LeadersViewData>().LeadersView;
            var leadersConfigData = _dataWorld.OneData<LeadersConfigData>();
            
            leadersConfigData.LeadersConfig.TryGetValue(selectLeaderData.SelectLeader, out var leadersConfig);
            leadersView.TryGetValue(leadersConfig.imageAvatarLeader, out var imAvatar);
            
            var entity = _dataWorld.NewEntity();

            entity.AddComponent(new PlayerComponent {
                playerTypeEnum = selectLeaderData.playerTypeEnum,
                PlayerID = selectLeaderData.PlayerID, 
                UnitCount = config.StartCountSquad, 
                VictoryPoint = 0,
                UnitAgentCountInHand = config.CountAgentPlayer,
                PositionInTurnQueue = positionInTurnQueue
            });

            entity.AddComponent(new PlayerNotInstallFirstBaseComponent());

            entity.AddComponent(new PlayerViewComponent 
            {
                LeaderKey = selectLeaderData.SelectLeader, 
                Name = selectLeaderData.NamePlayer,
                Avatar = imAvatar,
                KeyCityVisual = selectLeaderData.KeyVisualCity
            });
        }
    }
}