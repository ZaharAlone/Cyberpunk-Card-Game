using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class LoadingGameScreenSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            LoadingGameScreenAction.OpenLoadingGameScreen += OpenLoadingGameScreen;
            LoadingGameScreenAction.CloseLoadingGameScreen += CloseLoadingGameScreen;
        }
        
        private void OpenLoadingGameScreen()
        {
            ref var uiVSScreen = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.loadingGameScreenUIMono;
            uiVSScreen.OpenWindow();
        }
        
        private void CloseLoadingGameScreen()
        {
            _dataWorld.OneData<MetaUIData>().MetaUIMono.loadingGameScreenUIMono.CloseWindow();
        }
    }
}