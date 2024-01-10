using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Meta.StartGame
{
    [EcsSystem(typeof(CoreModule))]
    public class StartCampaignSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            StartGameAction.StartCampaign += StartCampaign;
        }
        
        private void StartCampaign()
        {
            Debug.LogError("Start Campaign");   
        }
    }
}