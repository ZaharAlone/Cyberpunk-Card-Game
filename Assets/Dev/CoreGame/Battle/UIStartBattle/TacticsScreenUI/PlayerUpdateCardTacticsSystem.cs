using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;

namespace CyberNet.Core.Battle.TacticsMode
{
    [EcsSystem(typeof(CoreModule))]
    public class PlayerUpdateCardTacticsSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.UpdateCardAndTactics += UpdateCardAndTactics;
        }
        
        private void UpdateCardAndTactics()
        {
            var currentPlayerEntity = _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirstEntity();

            var selectCardScreenTacticsQuery = _dataWorld.Select<CardTacticsComponent>()
                .With<CardSelectInTacticsScreenComponent>();

            if (selectCardScreenTacticsQuery.Count() != 0)
            {
                var selectCardTacticsComponent = new SelectTacticsAndCardComponent();

                if (currentPlayerEntity.HasComponent<SelectTacticsAndCardComponent>())
                    currentPlayerEntity.RemoveComponent<SelectTacticsAndCardComponent>();

                var selectCardComponent = selectCardScreenTacticsQuery.SelectFirstEntity().GetComponent<CardComponent>();

                selectCardTacticsComponent.KeyCard = selectCardComponent.Key;
                selectCardTacticsComponent.GUIDCard = selectCardComponent.GUID;

                var selectCurrentTactics = currentPlayerEntity.GetComponent<OpenBattleTacticsUIComponent>().CurrentSelectTacticsUI;
                selectCardTacticsComponent.BattleTacticsKey = selectCurrentTactics;

                currentPlayerEntity.AddComponent(selectCardTacticsComponent);
            }
            else
            {
                if (currentPlayerEntity.HasComponent<SelectTacticsAndCardComponent>())
                    currentPlayerEntity.RemoveComponent<SelectTacticsAndCardComponent>();
            }
            
            BattleTacticsUIAction.UpdateCurrencyPlayerInBattle?.Invoke();
        }

        public void Destroy()
        {
            BattleTacticsUIAction.UpdateCardAndTactics -= UpdateCardAndTactics;
        }
    }
}