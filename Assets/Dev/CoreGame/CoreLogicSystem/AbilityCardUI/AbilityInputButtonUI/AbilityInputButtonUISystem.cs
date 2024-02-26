using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;

namespace CyberNet.Core.AbilityCard.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityInputButtonUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityInputButtonUIAction.ShowCancelButton += ShowCancelButton;
            AbilityInputButtonUIAction.ShowTakeDamageBattleButton += ShowTakeDamageBattleButton;
            AbilityInputButtonUIAction.HideInputUIButton += HideCancelButton;
        }

        private void ShowCancelButton(bool isShowUp)
        {
            var supportLocalize = _dataWorld.OneData<BoardGameData>().SupportLocalize;
            var abilityInputButtonUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityInputButtonUIMono;
            abilityInputButtonUI.SetView("", supportLocalize.CancelButton, "", isShowUp);
            abilityInputButtonUI.EnableButton();
        }
        
        private void ShowTakeDamageBattleButton()
        {
            var supportLocalize = _dataWorld.OneData<BoardGameData>().SupportLocalize;
            var abilityInputButtonUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityInputButtonUIMono;
            abilityInputButtonUI.SetView(supportLocalize.HeaderTakeDamage, supportLocalize.TakeDamage, supportLocalize.DiscardCard);
            abilityInputButtonUI.EnableButton();
        }

        private void HideCancelButton()
        {
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityInputButtonUIMono.DisableButton();
        }

        public void Destroy()
        {
            AbilityInputButtonUIAction.ShowCancelButton -= ShowCancelButton;
            AbilityInputButtonUIAction.ShowTakeDamageBattleButton -= ShowTakeDamageBattleButton;
            AbilityInputButtonUIAction.HideInputUIButton -= HideCancelButton;
        }
    }
}