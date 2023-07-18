using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Meta.Leaders
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectLeaders : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectLeaderAction.OpenSelectLeader += OpenSelectLeader;
            SelectLeaderAction.SelectLeader += SelectLeaderView;
        }
        
        private void SelectLeaderView(string nameLeader)
        {
            
        }
        
        private void OpenSelectLeader()
        {
            
        }
    }
}