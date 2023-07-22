using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Global;

namespace CyberNet.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class SelectLeadersUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectLeaderAction.OpenSelectLeaderUI += OpenSelectLeaderUI;
            SelectLeaderAction.SelectLeader += SelectLeaderView;
            SelectLeaderAction.BackMainMenu += BackMainMenu;
            SelectLeaderAction.StartGame += StartGame;
            SelectLeaderAction.InitButtonLeader += InitButtonLeader;
        }
        private void StartGame()
        {
            var entitySelectLeader = _dataWorld.Select<SelectLeadersComponent>().SelectFirstEntity();
            var componentSelectLeader = entitySelectLeader.GetComponent<SelectLeadersComponent>();

            switch (componentSelectLeader.SelectGameMode)
            {
                case GameModeEnum.Campaign:
                    break;
                case GameModeEnum.LocalVSAI:
                    StartGameAction.StartGameLocalVSAI?.Invoke(componentSelectLeader.CurrentSelectLeader);
                    break;
                case GameModeEnum.LocalVSPlayer:
                    break;
                case GameModeEnum.OnlineGame:
                    break;
            }
            
            entitySelectLeader.Destroy();
            CloseSelectLeader();
        }

        private void SelectLeaderView(string nameLeader)
        {
            var leadersView = _dataWorld.OneData<LeadersViewData>().LeadersView;
            var leadersConfigData = _dataWorld.OneData<LeadersConfigData>();
            leadersConfigData.LeadersConfig.TryGetValue(nameLeader, out var leadersConfig);
            leadersConfigData.AbilityConfig.TryGetValue(leadersConfig.Ability, out var abilityConfig);

            leadersView.TryGetValue(leadersConfig.ImageCardLeaders, out var imCardLeaders);
            leadersView.TryGetValue(abilityConfig.ImageAbility, out var imAbility);
            
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.SetSelectViewLeader(imCardLeaders, leadersConfig.NameLoc, leadersConfig.DescrLoc);
            
            uiSelectLeader.SetSelectViewLeaderAbility(imAbility, abilityConfig.NameLoc, abilityConfig.DescrLoc);
            WriteInComponentSelectLeader(nameLeader);
        }

        private Sprite InitButtonLeader(string nameLeader, bool isFirstButton)
        {
            _dataWorld.OneData<LeadersConfigData>().LeadersConfig.TryGetValue(nameLeader, out var leaderConfig);
            _dataWorld.OneData<LeadersViewData>().LeadersView.TryGetValue(leaderConfig.ImageButtonLeader, out var imageButton);

            if (isFirstButton)
            {
                //TO-DO add select first button
                SelectLeaderView(nameLeader);
                WriteInComponentSelectLeader(nameLeader);
            }

            return imageButton;
        }

        private void WriteInComponentSelectLeader(string nameLeader)
        {
            var selectLeaderEntity = _dataWorld.Select<SelectLeadersComponent>().SelectFirstEntity();
            ref var selectLeaderComponent = ref selectLeaderEntity.GetComponent<SelectLeadersComponent>();
            selectLeaderComponent.CurrentSelectLeader = nameLeader;
        }
        
        private void OpenSelectLeaderUI(GameModeEnum gameModeEnum)
        {
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.OpenWindow();

            var entity = _dataWorld.NewEntity();
            entity.AddComponent(new SelectLeadersComponent {
                SelectGameMode = gameModeEnum
            });
        }

        private void BackMainMenu()
        {
            MainMenuAction.OpenMainMenu?.Invoke();
            _dataWorld.Select<SelectLeadersComponent>().SelectFirstEntity().Destroy();
            CloseSelectLeader();
        }

        private void CloseSelectLeader()
        {
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.CloseWindow();
        }
    }
}