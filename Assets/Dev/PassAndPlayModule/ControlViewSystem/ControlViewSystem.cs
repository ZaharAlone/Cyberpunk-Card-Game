using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;

namespace CyberNet.Core
{
    [EcsSystem(typeof(PassAndPlayModule))]
    public class ControlViewSystem : IActivateSystem, IPostRunEventSystem<EventDistributionCard>
    {
        private DataWorld _dataWorld;

        public void Activate()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            _dataWorld.CreateOneData(new ViewPlayerData { PlayerView = PlayerEnum.Player1 });
        }

        public void PostRunEvent(EventDistributionCard value)
        {
            SwitchView(value);
        }

        public void SwitchView(EventDistributionCard eventValue)
        {
            ref var viewData = ref _dataWorld.OneData<ViewPlayerData>();
            viewData.PlayerView = eventValue.Target;
        }
    }
}