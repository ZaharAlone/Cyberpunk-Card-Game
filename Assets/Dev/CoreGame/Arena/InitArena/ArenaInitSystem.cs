using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.City;
using CyberNet.Core.Player;
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
            CreateBattleData();
            CreateUnitInArena();
            EnableVisualArena();
            
            _dataWorld.InitModule<ArenaModule>(true);
        }

        private void CreateBattleData()
        {
            var unitEntities = _dataWorld.Select<UnitInBattleArenaComponent>().GetEntities();
            var unitDictionary = _dataWorld.OneData<BoardGameData>().CitySO.UnitDictionary;

            var playersInBattle = new List<PlayersInBattleStruct>();
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
                    playersInBattle.Add(new PlayersInBattleStruct {
                        PlayerID = unitComponent.PowerSolidPlayerID,
                        PlayerControl = unitComponent.PlayerControl,
                        Forwards = unitBattleComponent.Forwards
                    });
                }
            }

            foreach (var player in playersInBattle)
            {
                var visualKeyPlayer = GetKeyPlayerVisual(player.PlayerControl, player.PlayerID);
                unitDictionary.TryGetValue(visualKeyPlayer, out var visualPlayer);
                var avatar = GetAvatarPlayerVisual(player.PlayerControl, player.PlayerID);
                var positionInTurnQueue = GetPositionInTurnQueue(player.PlayerControl, player.PlayerID, playersInBattle.Count);
                    
                var playerBattleComponent = new PlayerArenaInBattleComponent
                {
                    PlayerID   = player.PlayerID,
                    PlayerControlEnum = player.PlayerControl,
                    Forwards = player.Forwards,
                    KeyCityVisual = visualKeyPlayer,
                    ColorVisual = visualPlayer.ColorUnit,
                    Avatar = avatar,
                    PositionInTurnQueue = positionInTurnQueue
                };
                _dataWorld.NewEntity().AddComponent(playerBattleComponent);
            }
        }
        
        private void CreateUnitInArena()
        {
            var arenaData = _dataWorld.OneData<ArenaData>();
            var unitEntities = _dataWorld.Select<UnitMapComponent>()
                .With<UnitInBattleArenaComponent>()
                .GetEntities();

            var unitDictionary = _dataWorld.OneData<BoardGameData>().CitySO.UnitDictionary;

            var indexUnit = 0;
            foreach (var unitEntity in unitEntities)
            {
                var unitMapComponent = unitEntity.GetComponent<UnitMapComponent>();
                var unitInBattleComponent = unitEntity.GetComponent<UnitInBattleArenaComponent>();
                var visualKeyUnit = GetKeyPlayerVisual(unitMapComponent.PlayerControl, unitMapComponent.PowerSolidPlayerID);

                unitDictionary.TryGetValue(visualKeyUnit, out var unitVisual);
                var unitArenaMono = Object.Instantiate(unitVisual.UnitArenaMono);
                unitArenaMono.GUID = unitMapComponent.GUIDUnit;
                
                var unitArenaComponent = new ArenaUnitComponent
                {
                    UnitArenaMono = unitArenaMono,
                    PlayerControlID = unitMapComponent.PowerSolidPlayerID,
                    PlayerControlEnum = unitMapComponent.PlayerControl,
                    UnitGO = unitArenaMono.gameObject,
                    GUID = unitMapComponent.GUIDUnit,
                    IndexTurnOrder = indexUnit
                };

                unitEntity.AddComponent(unitArenaComponent);
                arenaData.ArenaMono.InitUnitInPosition(unitArenaMono, unitInBattleComponent.Forwards);
                indexUnit++;
            }
        }
        
        public string GetKeyPlayerVisual(PlayerControlEnum playerControlEnum, int playerID)
        {
            var visualKeyUnit = "";
            if (playerControlEnum == PlayerControlEnum.Neutral)
            {
                visualKeyUnit = "neutral_unit";
            }
            else
            {
                var playerEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == playerID)
                    .SelectFirstEntity();

                var playerViewComponent = playerEntity.GetComponent<PlayerViewComponent>();
                    
                visualKeyUnit = playerViewComponent.KeyCityVisual;
            }
            return visualKeyUnit;
        }
        
        public Sprite GetAvatarPlayerVisual(PlayerControlEnum playerControlEnum, int playerID)
        {
            Sprite avatar;
            if (playerControlEnum == PlayerControlEnum.Neutral)
            {
                _dataWorld.OneData<LeadersViewData>().LeadersView.TryGetValue("im_avatar_neutral", out avatar);
            }
            else
            {
                var playerEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == playerID)
                    .SelectFirstEntity();

                var playerViewComponent = playerEntity.GetComponent<PlayerViewComponent>();
                avatar = playerViewComponent.Avatar;
            }
            return avatar;
        }

        public int GetPositionInTurnQueue(PlayerControlEnum playerControlEnum, int playerID, int countUnitInBattle)
        {
            var position = -1;
            if (playerControlEnum == PlayerControlEnum.Neutral)
            {
                position = countUnitInBattle - 1;
            }
            else
            {
                var playerEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == playerID)
                    .SelectFirstEntity();

                var playerComponent = playerEntity.GetComponent<PlayerComponent>();
                position = playerComponent.PositionInTurnQueue;
            }
            return position;
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
            
            _dataWorld.RemoveOneData<PlayerArenaInBattleComponent>();
            _dataWorld.DestroyModule<ArenaModule>();
        }
        
        public void Destroy()
        {
            ArenaAction.StartArenaBattle -= StartArenaBattle;
            ArenaAction.DisableArenaBattle -= DisableArena;
            
            Object.Destroy(_dataWorld.OneData<ArenaData>().ArenaMono.gameObject);
            _dataWorld.RemoveOneData<ArenaData>();
        }
    }
}