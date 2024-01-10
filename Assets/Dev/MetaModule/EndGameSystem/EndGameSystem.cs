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
            var isLocal = _dataWorld.IsModuleActive<LocalGameModule>();
            var isServer = _dataWorld.IsModuleActive<ServerModule>();

            if (isLocal)
            {
                _dataWorld.DestroyModule<LocalGameModule>();
            }
            else
            {
                _dataWorld.DestroyModule<ServerModule>();
            }
            
            _dataWorld.DestroyModule<CoreModule>();
            _dataWorld.OneData<MetaUIData>().MetaUIMono.MainMenuUIMono.OpenMainMenu();
        }
    }
}