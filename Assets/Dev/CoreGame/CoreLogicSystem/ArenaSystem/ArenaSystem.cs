using CyberNet.Core.UI;
using CyberNet.Global.GameCamera;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaSystem : IPreInitSystem, IInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.StartArenaBattle += StartArenaBattle;
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

        public void ControlViewGameUI(bool isOpenArena)
        {
            var uiCoreMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            
            if (isOpenArena)
            {
                uiCoreMono.TraderowMono.HideTradeRow();
                uiCoreMono.CoreHudUIMono.HideEnemyPassport();
            }
            else
            {
                uiCoreMono.TraderowMono.ShowTradeRow();
                uiCoreMono.CoreHudUIMono.ShowEnemyPassport();
            }
        }
        
        private void StartArenaBattle()
        {
            var arenaData = _dataWorld.OneData<ArenaData>();
            var cameraData = _dataWorld.OneData<GameCameraData>();
            var cityData = _dataWorld.OneData<CityData>();
            
            arenaData.ArenaMono.EnableArena();
            arenaData.ArenaMono.EnableCamera();
            cameraData.CameraGO.SetActive(false);
            cityData.CityMono.OffCityLight();
            
            ControlViewGameUI(true);
            Debug.LogError("Start Arena battle");
        }
        

        private void DisableArena()
        {
            var arenaData = _dataWorld.OneData<ArenaData>();
            var cameraData = _dataWorld.OneData<GameCameraData>();
            var cityData = _dataWorld.OneData<CityData>();
            
            arenaData.ArenaMono.DisableArena();
            arenaData.ArenaMono.DisableCamera();
            cameraData.CameraGO.SetActive(true);
            cityData.CityMono.OnCityLight();
        }

        public void Run() { }
    }
}