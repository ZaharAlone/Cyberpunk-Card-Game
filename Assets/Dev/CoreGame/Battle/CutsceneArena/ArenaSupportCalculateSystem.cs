using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.Map;
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
        
        /*
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

                var visualKeyUnit = CutsceneArenaAction.GetKeyPlayerVisual.Invoke(unitMapComponent.PlayerControl, unitMapComponent.PowerSolidPlayerID);

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
        }*/

        public void Destroy()
        {
        }
    }
}