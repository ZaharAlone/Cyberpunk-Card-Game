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

        private void ShowCancelButton()
        {
            var supportLocalize = _dataWorld.OneData<BoardGameData>().SupportLocalize;
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityInputButtonUIMono.SetView("", supportLocalize.CancelButton, "");
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityInputButtonUIMono.EnableButton();
        }
        
        private void ShowTakeDamageBattleButton()
        {
            var supportLocalize = _dataWorld.OneData<BoardGameData>().SupportLocalize;
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityInputButtonUIMono.SetView(supportLocalize.HeaderTakeDamage, supportLocalize.TakeDamage, supportLocalize.DiscardCard);
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityInputButtonUIMono.EnableButton();
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