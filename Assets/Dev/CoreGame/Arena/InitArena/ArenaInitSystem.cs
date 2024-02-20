using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.City;
using CyberNet.Core.UI;
using CyberNet.Global.GameCamera;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaInitSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.StartArenaBattle += StartArenaBattle;
            ArenaAction.EndBattleArena += EndBattleArena;
        }
        
        public void Init()
        {
            var arenaMonoPrefab = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ArenaMono;
            var arenaMonoInit = Object.Instantiate(arenaMonoPrefab);
            arenaMonoInit.transform.position = new Vector3(-150, 0, 200);
            
            var arenaData = new ArenaData {
                ArenaMono = arenaMonoInit
            };
            
            _dataWorld.CreateOneData(arenaData);
        }
        
        private void StartArenaBattle()
        {
            ArenaAction.CreateBattleData?.Invoke();
            ArenaAction.CreateUnitInArena?.Invoke();
            EnableVisualArena();
            
            _dataWorld.InitModule<ArenaModule>(true);
        }
        
        private void EnableVisualArena()
        {
            var arenaData = _dataWorld.OneData<ArenaData>();
            var cameraData = _dataWorld.OneData<GameCameraData>();
            var cityData = _dataWorld.OneData<CityData>();
            
            arenaData.ArenaMono.EnableArena();
            arenaData.ArenaMono.EnableCamera();
            cameraData.CameraGO.SetActive(false);
            cityData.CityMono.OffCityLight();
            
            ControlViewGameUI(true);
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
        }
        
        public void ControlViewGameUI(bool isOpenArena)
        {
            var uiCoreMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            
            if (isOpenArena)
            {
                uiCoreMono.TraderowMono.HideTradeRow();
                uiCoreMono.CoreHudUIMono.HideEnemyPassport();
                uiCoreMono.ArenaHUDUIMono.OnArenaHUD();
            }
            else
            {
                uiCoreMono.TraderowMono.ShowTradeRow();
                uiCoreMono.CoreHudUIMono.ShowEnemyPassport();
                uiCoreMono.ArenaHUDUIMono.OffArenaHUD();
            }
        }
        
        private void EndBattleArena()
        {
            var arenaData = _dataWorld.OneData<ArenaData>();
            var cameraData = _dataWorld.OneData<GameCameraData>();
            var cityData = _dataWorld.OneData<CityData>();

            ControlViewGameUI(false);
            AnimationsStartArenaCameraAction.ReturnCamera?.Invoke();
            arenaData.ArenaMono.DisableArena();
            arenaData.ArenaMono.DisableCamera();
            cameraData.CameraGO.SetActive(true);
            cityData.CityMono.OnCityLight();

            var playerInArenaEntities = _dataWorld.Select<PlayerArenaInBattleComponent>().GetEntities();
            
            foreach (var playerEntity in playerInArenaEntities)
            {
                playerEntity.Destroy();
            }

            var unitInArenaEntities = _dataWorld.Select<ArenaUnitComponent>().GetEntities();

            foreach (var unitEntity in unitInArenaEntities)
            {
                var unitComponent = unitEntity.GetComponent<ArenaUnitComponent>();
                Object.Destroy(unitComponent.UnitGO);
                unitEntity.RemoveComponent<ArenaUnitComponent>();
                unitEntity.RemoveComponent<ArenaUnitCurrentComponent>();
                unitEntity.RemoveComponent<UnitInBattleArenaComponent>();
            }

            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = false;
            roundData.CurrentRoundState = RoundState.Map;
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            
            _dataWorld.DestroyModule<ArenaModule>();
        }
        
        public void Destroy()
        {
            ArenaAction.StartArenaBattle -= StartArenaBattle;
            ArenaAction.EndBattleArena -= EndBattleArena;
            
            Object.Destroy(_dataWorld.OneData<ArenaData>().ArenaMono.gameObject);
            _dataWorld.RemoveOneData<ArenaData>();

            var arenaUnitEntities = _dataWorld.Select<ArenaUnitComponent>().GetEntities();
            foreach (var entity in arenaUnitEntities)
            {
                entity.Destroy();
            }
            
            var playerInArenaEntities = _dataWorld.Select<PlayerArenaInBattleComponent>().GetEntities();
            foreach (var entity in playerInArenaEntities)
            {
                entity.Destroy();
            }
        }
    }
}