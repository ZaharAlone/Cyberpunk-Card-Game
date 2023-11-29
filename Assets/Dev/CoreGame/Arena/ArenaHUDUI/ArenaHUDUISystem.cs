using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
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
            
        }
        
        public void Init()
        {
            InitStartVisual();
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

        public void Destroy()
        {
            
        }
    }
}