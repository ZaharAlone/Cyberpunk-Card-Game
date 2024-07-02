using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.Arena.Support;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaSupportCalculateSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            ArenaAction.CheckFinishArenaBattle += CheckEndRound;
            ArenaAction.UpdateTurnOrderArena += UpdateTurnOrderArena;
            ArenaAction.FindPlayerInCurrentRound += FindPlayerInCurrentRound;
            ArenaAction.CreateBattleData += CreateBattleData;
            ArenaAction.GetKeyPlayerVisual += GetKeyPlayerVisual;
            ArenaAction.CreateUnitInArena += CreateUnitInArena;
            ArenaAction.DeselectPlayer += DeselectPlayer;
        }

        private bool CheckEndRound()
        {
            ClearPlayersNotUnit();
            
            var playersCountInArena = _dataWorld.Select<PlayerArenaInBattleComponent>().Count();
            if (playersCountInArena == 1)
                return true;
            
            var forwardsArenaPlayerID = 0;
            
            var playersInBattleEntities = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .GetEntities();

            foreach (var playerEntity in playersInBattleEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerArenaInBattleComponent>();
                if (playerComponent.Forwards)
                {
                    forwardsArenaPlayerID = playerComponent.PlayerID;
                    break;
                }
            }
            
            var countForwardsUnit = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == forwardsArenaPlayerID)
                .Count();

            return countForwardsUnit == 0;
        }
        
        private void ClearPlayersNotUnit()
        {
            var playerEntities = _dataWorld.Select<PlayerArenaInBattleComponent>().GetEntities();
            
            foreach (var playerEntity in playerEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerArenaInBattleComponent>();

                var countUnit = _dataWorld.Select<ArenaUnitComponent>()
                    .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == playerComponent.PlayerID)
                    .Count();
                if (countUnit == 0)
                    playerEntity.Destroy();
            }
        }

        private void UpdateTurnOrderArena()
        {
            var playersInBattleEntities = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .GetEntities();
            
            var playersCountInBattle = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Count();

            foreach (var playerEntity in playersInBattleEntities)
            {
                ref var playerComponent = ref playerEntity.GetComponent<PlayerArenaInBattleComponent>();
                playerComponent.PositionInTurnQueue--;

                if (playerComponent.PositionInTurnQueue < 0)
                {
                    playerComponent.PositionInTurnQueue = playersCountInBattle -1;
                }
            }
        }
        
        private void FindPlayerInCurrentRound()
        {
            ref var roundData = ref _dataWorld.OneData<ArenaRoundData>();
            var playersInBattleEntities = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .GetEntities();

            var positionInTurnQueue = 50;
            var newCurrentPlayerID = 0;
            
            foreach (var playerEntity in playersInBattleEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerArenaInBattleComponent>();
                if (playerComponent.PositionInTurnQueue < positionInTurnQueue)
                {
                    positionInTurnQueue = playerComponent.PositionInTurnQueue;
                    newCurrentPlayerID = playerComponent.PlayerID;
                }
            }
            
            var playerEntityCurrentRound = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Where<PlayerArenaInBattleComponent>(player => player.PlayerID == newCurrentPlayerID)
                .SelectFirstEntity();

            var playerArenaComponent = playerEntityCurrentRound.GetComponent<PlayerArenaInBattleComponent>();
            
            roundData.PlayerControlEntity = playerArenaComponent.PlayerControlEntity;
            roundData.CurrentPlayerID = playerArenaComponent.PlayerID;
            
            playerEntityCurrentRound.AddComponent(new CurrentPlayerComponent());
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

            var listPlayersPositionInTurnQueue = SortingPlayersInTurn(playersInBattle);

            foreach (var player in playersInBattle)
            {
                var visualKeyPlayer = GetKeyPlayerVisual(player.PlayerControlEntity, player.PlayerID);
                unitDictionary.TryGetValue(visualKeyPlayer, out var visualPlayer);
                var avatar = GetAvatarPlayerVisual(player.PlayerControlEntity, player.PlayerID);
                var positionInTurnQueue = listPlayersPositionInTurnQueue.Find(playerInList => playerInList.PlayerID == player.PlayerID).PositionInTurnQueue;
                    
                var playerBattleComponent = new PlayerArenaInBattleComponent
                {
                    PlayerID   = player.PlayerID,
                    PlayerControlEntity = player.PlayerControlEntity,
                    Forwards = player.Forwards,
                    KeyCityVisual = visualKeyPlayer,
                    ColorVisual = visualPlayer.ColorUnit,
                    Avatar = avatar,
                    PositionInTurnQueue = positionInTurnQueue,
                };
                _dataWorld.NewEntity().AddComponent(playerBattleComponent);
            }
        }

        private string GetKeyPlayerVisual(PlayerControlEntity PlayerControlEntity, int playerID)
        {
            var visualKeyUnit = "";
            if (PlayerControlEntity == PlayerControlEntity.NeutralUnits)
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

        private Sprite GetAvatarPlayerVisual(PlayerControlEntity PlayerControlEntity, int playerID)
        {
            Sprite avatar;
            if (PlayerControlEntity == PlayerControlEntity.NeutralUnits)
            {
                _dataWorld.OneData<LeadersViewData>().LeadersView.TryGetValue("im_avatar_neutral", out avatar);
            }
            else
            {
                var playerEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == playerID)
                    .SelectFirstEntity();

                var playerViewComponent = playerEntity.GetComponent<PlayerViewComponent>();
                avatar = playerViewComponent.AvatarWithBackground;
            }
            return avatar;
        }
        
        private List<PlayersPositionInTurnQueue> SortingPlayersInTurn(List<PlayersInBattleStruct> playersInBattle)
        {
            var playersPositionInTurnQueue = new List<PlayersPositionInTurnQueue>();
            var positionUnit = 1;

            //var currentRoundPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            
            foreach (var player in playersInBattle)
            {
                if (player.PlayerControlEntity == PlayerControlEntity.NeutralUnits)
                {
                    playersPositionInTurnQueue.Add(new PlayersPositionInTurnQueue()
                    {
                        PlayerID = player.PlayerID,
                        PositionInTurnQueue = playersInBattle.Count - 1,
                    });
                }
                else
                {
                    var playerComponent = _dataWorld.Select<PlayerComponent>()
                        .Where<PlayerComponent>(playerComponent => playerComponent.PlayerID == player.PlayerID)
                        .SelectFirstEntity();
                    var currentPlayerPosition = positionUnit;

                    if (playerComponent.HasComponent<CurrentPlayerComponent>())
                        currentPlayerPosition = 0;
                    else
                        positionUnit++;
                    
                    playersPositionInTurnQueue.Add(new PlayersPositionInTurnQueue()
                    {
                        PlayerID = player.PlayerID,
                        PositionInTurnQueue = currentPlayerPosition,
                    });
                }
            }
            
            return playersPositionInTurnQueue;
        }

        private void CreateUnitInArena()
        {
            var arenaData = _dataWorld.OneData<ArenaData>();
            var unitEntities = _dataWorld.Select<UnitMapComponent>()
                .With<UnitInBattleArenaComponent>()
                .GetEntities();

            var unitDictionary = _dataWorld.OneData<BoardGameData>().CitySO.UnitDictionary;
            var isVisual = _dataWorld.OneData<ArenaData>().IsShowVisualBattle;
            
            var indexUnit = 0;
            foreach (var unitEntity in unitEntities)
            {
                var unitMapComponent = unitEntity.GetComponent<UnitMapComponent>();
                var unitInBattleComponent = unitEntity.GetComponent<UnitInBattleArenaComponent>();
                var visualKeyUnit = ArenaAction.GetKeyPlayerVisual.Invoke(unitMapComponent.PlayerControl, unitMapComponent.PowerSolidPlayerID);

                unitDictionary.TryGetValue(visualKeyUnit, out var unitVisual);

                var unitArenaComponent = new ArenaUnitComponent
                {
                    PlayerControlID = unitMapComponent.PowerSolidPlayerID,
                    PlayerControlEntity = unitMapComponent.PlayerControl,
                    GUID = unitMapComponent.GUIDUnit,
                    IndexTurnOrder = indexUnit
                };
                
                if (isVisual)
                {
                    var unitArenaMono = Object.Instantiate(unitVisual.UnitArenaMono);
                    unitArenaMono.GUID = unitMapComponent.GUIDUnit;

                    unitArenaComponent.UnitArenaMono = unitArenaMono;
                    unitArenaComponent.UnitGO = unitArenaMono.gameObject;
                    arenaData.ArenaMono.InitUnitInPosition(unitArenaMono, unitInBattleComponent.Forwards);
                }

                unitEntity.AddComponent(unitArenaComponent);
                indexUnit++;
            }
        }

        private void DeselectPlayer()
        {
            var unitsEntities = _dataWorld.Select<ArenaUnitComponent>()
                .GetEntities();
            var isVisual = _dataWorld.OneData<ArenaData>().IsShowVisualBattle;
            
            foreach (var unitEntity in unitsEntities)
            {
                var unitComponent = unitEntity.GetComponent<ArenaUnitComponent>();
                if(isVisual)
                    unitComponent.UnitArenaMono.UnitPointVFXMono.DisableEffect();

                if (unitEntity.HasComponent<ArenaUnitCurrentComponent>())
                    unitEntity.RemoveComponent<ArenaUnitCurrentComponent>();
            }

            var isEntityCurrentPlayer = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .With<CurrentPlayerComponent>()
                .TrySelectFirstEntity(out var currentPlayerEntity);

            if (isEntityCurrentPlayer)
                currentPlayerEntity.RemoveComponent<CurrentPlayerComponent>();
        }

        public void Destroy()
        {
            ArenaAction.CheckFinishArenaBattle -= CheckEndRound;
            ArenaAction.UpdateTurnOrderArena -= UpdateTurnOrderArena;
            ArenaAction.FindPlayerInCurrentRound -= FindPlayerInCurrentRound;
            ArenaAction.CreateBattleData -= CreateBattleData;
            ArenaAction.GetKeyPlayerVisual -= GetKeyPlayerVisual;
            ArenaAction.CreateUnitInArena -= CreateUnitInArena;
            ArenaAction.DeselectPlayer -= DeselectPlayer;
        }
    }
}