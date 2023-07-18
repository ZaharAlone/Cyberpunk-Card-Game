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
                    StartGameAction.StartGameLocalVSAI?.Invoke();
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
            CloseSelectLeader();
        }

        private void CloseSelectLeader()
        {
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.CloseWindow();
        }
    }
}