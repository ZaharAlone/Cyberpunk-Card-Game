using System.Collections.Generic;
using System.Linq;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Map;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core.Battle.TacticsMode
{
    [EcsSystem(typeof(CoreModule))]
    public class NeutralSelectCardTacticsSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const string kill_tactics_key = "KillKill";
        private const int max_add_power_card_neutral_unit = 1;
        private const int neutral_player_id = -1;
        
        public void PreInit()
        {
            BattleAction.SelectTacticsCardNeutralUnit += SelectTacticsCardNeutralUnit;
        }
        
        private void SelectTacticsCardNeutralUnit()
        {
            var neutralPlayerEntity = _dataWorld.Select<PlayerInBattleComponent>()
                .Where<PlayerInBattleComponent>(player => player.PlayerControlEntity == PlayerOrAI.None)
                .SelectFirstEntity();

            var neutralPlayerComponent = neutralPlayerEntity.GetComponent<PlayerInBattleComponent>();
            var enemyPlayerID = _dataWorld.Select<PlayerInBattleComponent>()
                .Where<PlayerInBattleComponent>(player => player.PlayerID != neutralPlayerComponent.PlayerID)
                .SelectFirst<PlayerInBattleComponent>().PlayerID;
            
            var isNeutralUnitsDistrict = BattleAction.CheckBattleFriendlyUnitsPresenceNeighboringDistrict.Invoke(neutral_player_id);
            var currentDistrictBattle = _dataWorld.OneData<BattleCurrentData>().DistrictBattleGUID;
            
            var selectTacticsKey = "";
            if (isNeutralUnitsDistrict)
            {
                var enemyMaxPower = BattleAction.CalculatePlayerMaxPower.Invoke(enemyPlayerID);
                var countNeutralUnits = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDDistrict == currentDistrictBattle
                        && unit.PlayerControl == PlayerControlEntity.NeutralUnits)
                    .Count();

                var listSuitableTactics = new List<string>();
                // Есть ли шанс на победу нейтральных юнитов
                if (enemyMaxPower <= countNeutralUnits + max_add_power_card_neutral_unit)
                    listSuitableTactics = FilterSuitableTactics(BattleCharacteristics.PowerPoint);
                else
                    listSuitableTactics = FilterSuitableTactics(BattleCharacteristics.KillPoint);

                var selectKeyIndex = Random.Range(0, listSuitableTactics.Count() - 1);
                selectTacticsKey = listSuitableTactics[selectKeyIndex];
            }
            else
            {
                selectTacticsKey = kill_tactics_key;
            }

            var neutralCardKey = _dataWorld.OneData<BattleTacticsData>().KeyNeutralBattleCard;
            var selectTactics = new SelectTacticsAndCardComponent {
                KeyCard = neutralCardKey,
                BattleTacticsKey = selectTacticsKey,
            };

            neutralPlayerEntity.AddComponent(selectTactics);
        }

        public List<string> FilterSuitableTactics(BattleCharacteristics targetCharacteristics)
        {
            var tacticsConfig = _dataWorld.OneData<BattleTacticsData>().BattleTactics;
            var listSuitableTactics = new List<string>();

            foreach (var battleTactics in tacticsConfig)
            {
                if (battleTactics.LeftCharacteristics == targetCharacteristics
                    || battleTactics.RightCharacteristics == targetCharacteristics)
                {
                    listSuitableTactics.Add(battleTactics.Key);
                }
            }
            return listSuitableTactics;
        }
        
        public void Destroy()
        {
            BattleAction.SelectTacticsCardNeutralUnit -= SelectTacticsCardNeutralUnit;
        }
    }
}