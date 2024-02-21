using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Meta.EndGame
{
    [EcsSystem(typeof(CoreModule))]
    public class EndGameSystem : IPreInitSystem//, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            EndGameAction.EndGame += EndGameLogic;
        }

        public void Destroy()
        {
            EndGameLogic();
        }
        
        private void EndGameLogic()
        {
            var isLocal = _dataWorld.IsModuleActive<LocalGameModule>();
            var isServer = _dataWorld.IsModuleActive<ServerModule>();

            if (isLocal)
            {
                _dataWorld.DestroyModule<LocalGameModule>();
            }
            else if (isServer)
            {
                _dataWorld.DestroyModule<ServerModule>();
            }
            
            _dataWorld.DestroyModule<CoreModule>();
            _dataWorld.OneData<MetaUIData>().MetaUIMono.MainMenuUIMono.OpenMainMenu();
        }
    }
}