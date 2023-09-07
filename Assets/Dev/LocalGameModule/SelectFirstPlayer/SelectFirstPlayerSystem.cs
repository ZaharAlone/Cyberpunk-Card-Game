using CyberNet.Core;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Local
{
    [EcsSystem(typeof(LocalGameModule))]
    public class SelectFirstPlayerSystem : IActivateSystem
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            var firstPlayer = SelectFirstPlayer.Select();
            ref var selectLeader = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            _dataWorld.CreateOneData(new RoundData { CurrentRound = 0, CurrentTurn = 1, CurrentPlayerID = selectLeader[0].PlayerID });
        }
    }
}