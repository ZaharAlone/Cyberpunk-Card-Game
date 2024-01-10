using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena.ArenaHUDUI
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaHUDUISystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaUIAction.ShowHUDButton += ShowHUDButton;
            ArenaUIAction.HideHUDButton += HideHUDButton;
            ArenaUIAction.StartNewRoundUpdateOrderPlayer += StartNewRoundUpdateOrderPlayer;
        }

        public void Init()
        {
            InitStartVisual();

            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.HideInteractiveButton();
            //Temp
            ShowHUDButton();
        }

        private void InitStartVisual()
        {
            var arenaConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ArenaConfigSO;
            var arenaUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ArenaHUDUIMono;

            var playersInBattleEntities = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .GetEntities();
            
            foreach (var playersInBattleEntity in playersInBattleEntities)
            {
                ref var playerBattleComponent = ref playersInBattleEntity.GetComponent<PlayerArenaInBattleComponent>();
                
                var playerSlot = Object.Instantiate(arenaConfig.ContainerAvatarUnitPrefab,
                    arenaUI.ContainerListCharacter);
                playerBattleComponent.ArenaContainerUICharacterMono = playerSlot;
                
                arenaUI.AddCharacterAvatars(playerSlot);
                
                playerSlot.SetVisualCharacter(playerBattleComponent.Avatar, playerBattleComponent.ColorVisual);
                playerSlot.PositionInTurnQueue = playerBattleComponent.PositionInTurnQueue;
            }

            arenaUI.UpdateOrderAvatarSlot();
        }
        
        private void ShowHUDButton()
        {
            var arenaUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ArenaHUDUIMono;
            arenaUI.ShowArenaUI();
        }
        
        private void HideHUDButton()
        {
            var arenaUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ArenaHUDUIMono;
            arenaUI.HideArenaUI();
        }

        private void StartNewRoundUpdateOrderPlayer()
        {
            var arenaUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ArenaHUDUIMono;
            arenaUI.UpdateOrderAvatarSlot();
        }

        public void Destroy()
        {
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.ShowInteractiveButton();
            _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.ArenaHUDUIMono.HideArenaUI();
            
            ArenaUIAction.ShowHUDButton -= ShowHUDButton;
            ArenaUIAction.HideHUDButton -= HideHUDButton;
            ArenaUIAction.StartNewRoundUpdateOrderPlayer -= StartNewRoundUpdateOrderPlayer;
        }
    }
}