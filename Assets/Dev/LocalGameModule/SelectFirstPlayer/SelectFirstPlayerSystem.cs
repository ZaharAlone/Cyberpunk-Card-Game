using CyberNet.Core;
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
            _dataWorld.CreateOneData(new RoundData { CurrentRound = 0, CurrentTurn = 1, CurrentPlayer = firstPlayer });
        }
    }
}