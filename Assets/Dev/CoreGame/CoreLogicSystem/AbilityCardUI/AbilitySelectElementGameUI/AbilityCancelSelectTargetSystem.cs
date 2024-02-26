using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI.TaskPlayerPopup;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityCancelSelectTargetSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            //AbilitySelectElementAction.CancelSelect += CancelSelect;
        }
        
        private void CancelSelect()
        {
            TaskPlayerPopupAction.HidePopup?.Invoke();
            
            var entitySelectAbilityTarget = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();
            ref var selectAbility = ref entitySelectAbilityTarget.GetComponent<AbilitySelectElementComponent>().AbilityCard;
            
            switch (selectAbility.AbilityType)
            {
                case AbilityType.DestroyCard:
                    break;
                case AbilityType.CloneCard:
                    break;
                case AbilityType.DestroyTradeCard:
                    break;
                case AbilityType.SwitchNeutralUnit:
                    break;
                case AbilityType.SwitchEnemyUnit:
                    break;
                case AbilityType.DestroyNeutralUnit:
                    break;
                case AbilityType.DestroyUnit:
                    break;
                case AbilityType.UnitMove:
                    break;
                case AbilityType.SetIce:
                    break;
                case AbilityType.DestroyIce:
                    break;
                case AbilityType.EnemyDiscardCard:
                    AbilitySelectElementAction.CancelSelectPlayer?.Invoke();
                    AbilityCardAction.CancelDiscardCard?.Invoke();
                    break;
            }
            
            //TODO Return card in hand, break animation move table card
            
            entitySelectAbilityTarget.RemoveComponent<AbilitySelectElementComponent>();
        }
    }
}