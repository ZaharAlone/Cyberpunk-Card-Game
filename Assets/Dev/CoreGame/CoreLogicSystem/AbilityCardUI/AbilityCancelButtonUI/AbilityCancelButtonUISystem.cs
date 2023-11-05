using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;

namespace CyberNet.Core.AbilityCard.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityCancelButtonUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCancelButtonUIAction.ShowCancelButton += ShowCancelButton;
            AbilityCancelButtonUIAction.HideCancelButton += HideCancelButton;
        }

        private void ShowCancelButton()
        {
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityCancelButtonUIMono.EnableButton();
        }
        
        private void HideCancelButton()
        {
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.AbilityCancelButtonUIMono.DisableButton();
        }
    }
}