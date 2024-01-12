using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
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
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.CurrentRoundState = RoundState.Arena;
            
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
                        PlayerControlEntity = unitComponent.PlayerControl,
                        Forwards = unitBattleComponent.Forwards
                    });
                }
            }

            foreach (var player in playersInBattle)
            {
                var visualKeyPlayer = GetKeyPlayerVisual(player.PlayerControlEntity, player.PlayerID);
                unitDictionary.TryGetValue(visualKeyPlayer, out var visualPlayer);
                var avatar = GetAvatarPlayerVisual(player.PlayerControlEntity, player.PlayerID);
                var positionInTurnQueue = GetPositionInTurnQueue(player.PlayerControlEntity, player.PlayerID, playersInBattle.Count);
                    
                var playerBattleComponent = new PlayerArenaInBattleComponent
                {
                    PlayerID   = player.PlayerID,
                    PlayerControlEntity = player.PlayerControlEntity,
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
                    playerControlEntity = unitMapComponent.PlayerControl,
                    UnitGO = unitArenaMono.gameObject,
                    GUID = unitMapComponent.GUIDUnit,
                    IndexTurnOrder = indexUnit
                };

                unitEntity.AddComponent(unitArenaComponent);
                arenaData.ArenaMono.InitUnitInPosition(unitArenaMono, unitInBattleComponent.Forwards);
                indexUnit++;
            }
        }
        
        public string GetKeyPlayerVisual(PlayerControlEntity PlayerControlEntity, int playerID)
        {
            var visualKeyUnit = "";
            if (PlayerControlEntity == PlayerControlEntity.Neutral)
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
        
        public Sprite GetAvatarPlayerVisual(PlayerControlEntity PlayerControlEntity, int playerID)
        {
            Sprite avatar;
            if (PlayerControlEntity == PlayerControlEntity.Neutral)
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

        public int GetPositionInTurnQueue(PlayerControlEntity PlayerControlEntity, int playerID, int countUnitInBattle)
        {
            var position = -1;
            if (PlayerControlEntity == PlayerControlEntity.Neutral)
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
        }
    }
}