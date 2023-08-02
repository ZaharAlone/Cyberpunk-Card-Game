using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Meta.EndGame
{
    [EcsSystem(typeof(CoreModule))]
    public class EndGameSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            EndGameAction.EndGame += EndGameLogic;
        }
        private void EndGameLogic()
        {
            var isVSAI = _dataWorld.IsModuleActive<VSAIModule>();
            var isPassAndPlay = _dataWorld.IsModuleActive<PassAndPlayModule>();
            var isServer = _dataWorld.IsModuleActive<ServerModule>();

            if (isVSAI)
            {
                EndVSAIGame();
            }
            else if (isPassAndPlay)
            {
                
            }
            else if (isServer)
            {
                
            }
        }

        private void EndVSAIGame()
        {
            _dataWorld.DestroyModule<LocalGameModule>();
            _dataWorld.DestroyModule<VSAIModule>();
            _dataWorld.DestroyModule<CoreModule>();
            _dataWorld.OneData<MetaUIData>().MetaUIMono.MainMenuUIMono.OpenMainMenu();
        }
    }
}