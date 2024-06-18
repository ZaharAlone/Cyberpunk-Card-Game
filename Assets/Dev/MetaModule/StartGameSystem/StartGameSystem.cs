using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Threading.Tasks;
using CyberNet.Server;
using ModulesFrameworkUnity;

namespace CyberNet.Meta.StartGame
{
    [EcsSystem(typeof(MetaModule))]
    public class StartGameSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            StartGameAction.StartLocalGame += StartLocalGame;
            StartGameAction.StartTutorial += StartTutorial;
        }

        private async void StartLocalGame()
        {
            LoadingGameScreenAction.OpenLoadingGameScreen?.Invoke();

            await Task.Delay(100);
            await _dataWorld.InitModuleAsync<LocalGameModule>(true);
            _dataWorld.InitModule<CoreModule>(true);
        }
        
        private async void StartTutorial()
        {
            LoadingGameScreenAction.OpenLoadingGameScreen?.Invoke();

            await Task.Delay(100);
            
            await _dataWorld.InitModuleAsync<TutorialGameModule>(true);
            await _dataWorld.InitModuleAsync<LocalGameModule>(true);
            _dataWorld.InitModule<CoreModule>(true);
        }
        
        private void OnlineGame()
        {
            ConnectServer();
        }
        
        private void ConnectServer()
        {
            ModulesUnityAdapter.world.InitModule<ServerModule>(true);
            ConnectServerAction.ConnectServer.Invoke();
        }

    }
}