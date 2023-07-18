using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using CyberNet.Server;

namespace CyberNet.Meta
{
    /// <summary>
    /// Система управляющая переключением окон в главном меню
    /// </summary>
    [EcsSystem(typeof(MetaModule))]
    public class MainMenuUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            MainMenuAction.OpenMainMenu += OpenMainMenu;
            MainMenuAction.OpenLocalGameVSAI += OpenLocalGameVSAI;
            MainMenuAction.OpenLocalGameVSPlayer += OpenLocalGameVSPlayer;
            MainMenuAction.OpenServerGame += OpenServerGame;
            MainMenuAction.OpenSettingsGame += OpenSettingsGame;
            MainMenuAction.OpenExitGame += OpenExitGame;
        }
        private void OpenExitGame()
        {
            throw new NotImplementedException();
        }
        private void OpenSettingsGame()
        {
            throw new NotImplementedException();
        }
        private void OpenServerGame()
        {
            throw new NotImplementedException();
        }
        private void OpenLocalGameVSPlayer()
        {
            throw new NotImplementedException();
        }
        private void OpenLocalGameVSAI()
        {
            
        }
        
        private void CloseMainMenu()
        {
            ref var metaUI = ref _dataWorld.OneData<MetaUIData>().MetaUIMono;
            metaUI.MainMenuUIMono.CloseMainMenu();
        }

        private void OpenMainMenu()
        {
            ref var metaUI = ref _dataWorld.OneData<MetaUIData>().MetaUIMono;
            metaUI.MainMenuUIMono.OpenMainMenu();
        }
        
        private void StartGameVSAI()
        {
            var menu = _dataWorld.OneData<MetaUIData>();
            menu.UIGO.SetActive(false);
            ModulesUnityAdapter.world.InitModule<LocalGameModule>(true);
            ModulesUnityAdapter.world.InitModule<VSAIModule>(true);
        }

        private void StartGamePassAndPlay()
        {
            var menu = _dataWorld.OneData<MetaUIData>();
            menu.UIGO.SetActive(false);
            ModulesUnityAdapter.world.InitModule<LocalGameModule>(true);
            ModulesUnityAdapter.world.InitModule<PassAndPlayModule>(true);
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
        
        public void Exti()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
		        Application.Quit();
#endif
        }

        public void GoToWebPage(string url)
        {
            Application.OpenURL(url);
        }
    }
}