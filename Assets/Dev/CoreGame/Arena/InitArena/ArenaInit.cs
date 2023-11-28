using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.City;
using CyberNet.Core.UI;
using CyberNet.Global.GameCamera;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaInit : IPreInitSystem, IInitSystem, IDeactivateSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.StartArenaBattle += StartArenaBattle;
            ArenaAction.DisableArenaBattle += DisableArena;
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
            _dataWorld.InitModule<ArenaModule>();

            CreateBattleData();
            CreateUnitInArean();
            EnableVisualArena();
        }

        private void CreateBattleData()
        {
            var unitEntities = _dataWorld.Select<UnitInBattleArenaComponent>().GetEntities();

            var playersInBattle = new List<PlayerInBattle>();
            foreach (var unit in unitEntities)
            {
                var unitComponent = unit.GetComponent<UnitMapComponent>();
                var unitBattleComponent = unit.GetComponent<UnitInBattleArenaComponent>();
                var isUnique = true;
                
                foreach (var playerInBattle in playersInBattle)
                {
                    if (playerInBattle.PlayerID == unitComponent.PowerSolidPlayerID)
                        isUnique = false;
                }

                if (isUnique)
                {
                    playersInBattle.Add(new PlayerInBattle
                    {
                        PlayerID   = unitComponent.PowerSolidPlayerID,
                        PlayerControlEnum = unitComponent.PlayerControl,
                        Forwards = unitBattleComponent.Forwards
                    });
                }
            }
            
            var arenaBattleData = new ArenaBattleData {PlayersInBattle = playersInBattle};
            
            _dataWorld.CreateOneData(arenaBattleData);
        }
        
        private void CreateUnitInArean()
        {
            
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
        
        private void DisableArena()
        {
            var arenaData = _dataWorld.OneData<ArenaData>();
            var cameraData = _dataWorld.OneData<GameCameraData>();
            var cityData = _dataWorld.OneData<CityData>();
            
            arenaData.ArenaMono.DisableArena();
            arenaData.ArenaMono.DisableCamera();
            cameraData.CameraGO.SetActive(true);
            cityData.CityMono.OnCityLight();
            
            _dataWorld.RemoveOneData<ArenaBattleData>();
        }
        
        public void Deactivate()
        {
            ArenaAction.StartArenaBattle -= StartArenaBattle;
            ArenaAction.DisableArenaBattle -= DisableArena;
            
            Object.Destroy(_dataWorld.OneData<ArenaData>().ArenaMono.gameObject);
            _dataWorld.RemoveOneData<ArenaData>();
        }
    }
}