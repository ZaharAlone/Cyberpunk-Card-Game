using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Global.BlackScreen
{
    [EcsSystem(typeof(CoreModule))]
    public class BlackScreenUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BlackScreenUIAction.ForceShowScreen += ForceShowScreen;
            BlackScreenUIAction.ForceHideScreen += ForceHideScreen;
            BlackScreenUIAction.AnimationsShowScreen += AnimationsShowScreen;
            BlackScreenUIAction.AnimationsHideScreen += AnimationsHideScreen;
        }
        
        private void ForceShowScreen()
        {
            _dataWorld.OneData<BlackScreenUIData>().BlackScreenUIMono.ForceShowScreen();
        }
        
        private void ForceHideScreen()
        {
            _dataWorld.OneData<BlackScreenUIData>().BlackScreenUIMono.ForceHideScreen();
        }
        
        private void AnimationsShowScreen()
        {
            _dataWorld.OneData<BlackScreenUIData>().BlackScreenUIMono.AnimationsShowScreen();
        }
        
        private void AnimationsHideScreen()
        {
            _dataWorld.OneData<BlackScreenUIData>().BlackScreenUIMono.AnimationsHideScreen();
        }

        public void Destroy()
        {
            BlackScreenUIAction.ForceShowScreen -= ForceShowScreen;
            BlackScreenUIAction.ForceHideScreen -= ForceHideScreen;
            BlackScreenUIAction.AnimationsShowScreen -= AnimationsShowScreen;
            BlackScreenUIAction.AnimationsHideScreen -= AnimationsHideScreen;
        }
    }
}