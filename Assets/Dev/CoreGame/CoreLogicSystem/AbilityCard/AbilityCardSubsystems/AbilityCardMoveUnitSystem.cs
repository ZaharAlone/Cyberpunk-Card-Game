using CyberNet.Core.AI;
using CyberNet.Core.City;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityCardMoveUnitSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.MoveUnit += MoveUnit;
        }
        
        private void MoveUnit()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                AbilityAIAction.MoveUnit?.Invoke();
                return;
            }

            roundData.PauseInteractive = true;
            AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(0);
            CityAction.ShowWhereIsMovePlayer?.Invoke();
            AbilityCardAction.ConfimSelectTower += ConfimSelectTower;
        }

        private void ConfimSelectTower(TowerMono towerMono)
        {
            
        }
    }
}