using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Global;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class KillUnitInMapSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float time_animations_kill_unit = 1f;
        
        public void PreInit()
        {
            BattleAction.KillUnitInMapView += KillUnitInMapView;
        }
        
        private void KillUnitInMapView()
        {
            var isKilledUnits = SelectAllUnitKilled();
            
            Debug.LogError($"is killed units in battle: {isKilledUnits}");
            if (isKilledUnits)
                ShowViewKilledUnit();
            else
                BattleAction.EndAnimationsKillUnitsInMap?.Invoke();
        }

        private bool SelectAllUnitKilled()
        {
            var killUnitEntities = _dataWorld.Select<NumberOfDeathsUnitsInBattleComponent>().GetEntities();
            var currentDistrictBattleGUID = _dataWorld.OneData<BattleCurrentData>().DistrictBattleGUID;

            var isKillUnit = false;
            
            foreach (var killUnitEntity in killUnitEntities)
            {
                var killUnitComponent = killUnitEntity.GetComponent<NumberOfDeathsUnitsInBattleComponent>();
                var playerInBattleComponent = killUnitEntity.GetComponent<PlayerInBattleComponent>();

                var playerUnitInTargetDistrictQuery = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDDistrict == currentDistrictBattleGUID
                        && unit.PowerSolidPlayerID == playerInBattleComponent.PlayerID);

                var playerUnitInTargetDistrictEntities = playerUnitInTargetDistrictQuery.GetEntities();
                var countKillUnit = killUnitComponent.CountUnitDeaths;
                
                //Если юнитам некуда переместиться, а они проиграли (о чем говорит соответствующий компонент)
                //Убиваем их всех
                if (killUnitEntity.HasComponent<NotZoneToRetreatLozingPlayerComponent>())
                    countKillUnit = playerUnitInTargetDistrictQuery.Count();
                
                foreach (var unitEntity in playerUnitInTargetDistrictEntities)
                {
                    if (countKillUnit <= 0)
                        break;

                    unitEntity.AddComponent(new UnitKilledComponent());
                    countKillUnit--;
                    isKillUnit = true;
                }

                killUnitEntity.RemoveComponent<NumberOfDeathsUnitsInBattleComponent>();
            }

            return isKillUnit;
        }

        private void ShowViewKilledUnit()
        {
            var killedUnitEntities = _dataWorld.Select<UnitKilledComponent>().GetEntities();

            foreach (var unitEntity in killedUnitEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                unitComponent.IconsUnitInMapMono.PlayVFXKillUnitInMap();
            }

            var waitEndAnimationsKillUnit = _dataWorld.NewEntity();
            waitEndAnimationsKillUnit.AddComponent(new TimeComponent {
                Time = time_animations_kill_unit,
                Action = () => DestroyAllKilledUnit(),
            });
        }

        private void DestroyAllKilledUnit()
        {
            var killedUnitEntities = _dataWorld.Select<UnitKilledComponent>().GetEntities();

            foreach (var unitEntity in killedUnitEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                Object.Destroy(unitComponent.IconsUnitInMapMono.gameObject);
                unitEntity.Destroy();
            }
            
            BattleAction.EndAnimationsKillUnitsInMap?.Invoke();
        }

        public void Destroy()
        {
            BattleAction.KillUnitInMapView -= KillUnitInMapView;
        }
    }
}