using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Map;
using CyberNet.Core.Player;

namespace CyberNet.Core.AI.Ability
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityMoveUnitAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private string _guidCard;
        public void PreInit()
        {
            AbilityAIAction.MoveUnit += AbilityMoveUnit;
            AbilityAIAction.CalculatePotentialMoveUnitAttack += CalculatePotentialMoveUnitAttack;
            AbilityAIAction.CalculatePotentialMoveUnit += CalculatePotentialMoveUnitOnItsTerritory;
        }

        private void AbilityMoveUnit(string guidCard)
        {
            _guidCard = guidCard;
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();
            
            MoveUnit();
                        
            entityCard.RemoveComponent<AbilitySelectElementComponent>();
            entityCard.RemoveComponent<AbilityCardMoveUnitComponent>();
        }
        
        private void MoveUnit()
        {
            var potentialAttack = CalculatePotentialMoveUnitAttack();
            if (potentialAttack.Value > 0)
            {
                AttackTower(potentialAttack);
                return;
            }

            Debug.LogError("Not Attack enemy");
            var potentialMoveMyTower = CalculatePotentialMoveUnitOnItsTerritory();
            
            if (potentialMoveMyTower.Value != 0)
            {
                
            }
            else
            {
                Debug.LogError("Некоректное применение абилки перемещение юнитов, применить абилку не является целесообразным");
            }
        }

        private ItemValue CalculatePotentialMoveUnitAttack()
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            var towerEntitiesPlayer = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.DistrictBelongPlayerID == currentPlayerComponent.PlayerID
                    && tower.PlayerControlEntity == PlayerControlEntity.PlayerControl)
                .GetEntities();
            
            // Смотрим соседей башни игрока, куда можно отправить юнитов
            var guidSelectPotentiallyWeakTower = new List<ItemValue>();
            foreach (var towerEntity in towerEntitiesPlayer)
            {
                var towerComponent = towerEntity.GetComponent<DistrictComponent>();
                
                foreach (var zoneConnectTower in towerComponent.DistrictMono.ZoneConnect)
                {
                    var connectTowerEntity = _dataWorld.Select<DistrictComponent>()
                        .Where<DistrictComponent>(tower => tower.GUID == zoneConnectTower.GUID)
                        .SelectFirstEntity();
                    var connectTowerComponent = connectTowerEntity.GetComponent<DistrictComponent>();

                    if (connectTowerComponent.DistrictBelongPlayerID != currentPlayerComponent.PlayerID)
                    {
                        var countUnitInTower = _dataWorld.Select<UnitMapComponent>()
                            .Where<UnitMapComponent>(unit => unit.PowerSolidPlayerID != currentPlayerComponent.PlayerID
                                && unit.GUIDTower == connectTowerComponent.GUID)
                            .Count();

                        var potentialDefenceTower = countUnitInTower;
                        if (connectTowerComponent.PlayerControlEntity == PlayerControlEntity.PlayerControl)
                            potentialDefenceTower += 5; // прибавляем базовый максимум карт на руке игрока, у нейтрального нет карт, так что не прибавляем

                        var potentialAttackAI = CalculatePotentialAttackToTower(connectTowerComponent);
                        var potentialAttack = potentialAttackAI - potentialDefenceTower;
                        
                        guidSelectPotentiallyWeakTower.Add(new ItemValue{Item = connectTowerComponent.GUID, Value = potentialAttack});
                        break;
                    }
                }
            }

            var maxValuePotentialAttack = 0;
            var selectIndex = 0;
            var index = 0;
            
            foreach (var towerPotential in guidSelectPotentiallyWeakTower)
            {
                if (towerPotential.Value > maxValuePotentialAttack)
                {
                    maxValuePotentialAttack = towerPotential.Value;
                    selectIndex = index;
                }
                index++;
            }
            
            return guidSelectPotentiallyWeakTower[selectIndex];
        }
        
        private int CalculatePotentialAttackToTower(DistrictComponent DistrictComponent)
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var countUnitForBattle = 0;
            foreach (var towerConnect in DistrictComponent.DistrictMono.ZoneConnect)
            {
                var countPlayerUnit = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerConnect.GUID
                        && unit.PowerSolidPlayerID == currentPlayerID)
                    .Count();

                var freeUnitForBattle = countPlayerUnit - 1;
                if (freeUnitForBattle < 0)
                    freeUnitForBattle = 0;

                countUnitForBattle += freeUnitForBattle;
            }

            var countCardInHand = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .Count();

            var potentialAttack = countUnitForBattle + countCardInHand;
            return potentialAttack;
        }

        private ItemValue CalculatePotentialMoveUnitOnItsTerritory()
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var towerEntitiesPlayer = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.DistrictBelongPlayerID == currentPlayerID
                    && tower.PlayerControlEntity == PlayerControlEntity.PlayerControl)
                .GetEntities();
            
            var selectPotentiallyTower = new List<ItemValue>();
            foreach (var towerEntity in towerEntitiesPlayer)
            {
                var towerComponent = towerEntity.GetComponent<DistrictComponent>();
                var countPlayerUnitInTower = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerComponent.GUID
                        && unit.PowerSolidPlayerID == currentPlayerID
                        && unit.PlayerControl == PlayerControlEntity.PlayerControl)
                    .Count();

                var isSelectTower = false;
                foreach (var towerConnect in towerComponent.DistrictMono.ZoneConnect)
                {
                    var countEnemyUnitInTower = _dataWorld.Select<UnitMapComponent>()
                        .Where<UnitMapComponent>(unit => unit.GUIDTower == towerConnect.GUID
                            && unit.PowerSolidPlayerID != currentPlayerID)
                        .Count();

                    if (countPlayerUnitInTower < countEnemyUnitInTower)
                    {
                        isSelectTower = true;
                        break;
                    }
                }

                if (isSelectTower)
                {
                    foreach (var towerConnect in towerComponent.DistrictMono.ZoneConnect)
                    {
                        var countPlayerNeighboringUnitInTower = _dataWorld.Select<UnitMapComponent>()
                            .Where<UnitMapComponent>(unit => unit.GUIDTower == towerConnect.GUID
                                && unit.PowerSolidPlayerID == currentPlayerID
                                && unit.PlayerControl == PlayerControlEntity.PlayerControl)
                            .Count();

                        if (countPlayerNeighboringUnitInTower - 2 > countPlayerUnitInTower)
                        {
                            selectPotentiallyTower.Add(new ItemValue {Item = towerComponent.GUID, Value = countPlayerUnitInTower});
                            break;
                        }
                    }
                }
            }

            if (selectPotentiallyTower.Count == 0)
                return new ItemValue();
            else
            {
                var targetTower = new ItemValue();
                var minUnit = 0;
                
                foreach (var potentialTower in selectPotentiallyTower)
                {
                    if (potentialTower.Value > minUnit)
                    {
                        minUnit = potentialTower.Value;
                        targetTower = potentialTower;
                    }    
                }

                return targetTower;
            }
        }

        private void AttackTower(ItemValue targetTower)
        {
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == targetTower.Item)
                .SelectFirstEntity();
            var targetTowerComponent = towerEntity.GetComponent<DistrictComponent>();
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var needCountUnit = 0;

            if (targetTowerComponent.PlayerControlEntity == PlayerControlEntity.NeutralUnits)
                needCountUnit = 2;
            else
            {
                var countEnemyUnit = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == targetTowerComponent.GUID
                        && unit.PowerSolidPlayerID != currentPlayerID)
                    .Count();

                needCountUnit = countEnemyUnit + 5;
            }

            var countPlayerCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .With<CardHandComponent>()
                .Count();

            needCountUnit -= countPlayerCard;
            if (needCountUnit <= 0)
                needCountUnit = 1;
            
            var unitsForAttacks = new List<ItemValue>();
            var sumCountAddUnit = 0;
            
            // Выбираем каких сколько юнитов и с каких зон отправим в бой
            // Считает неверно
            foreach (var towerConnect in targetTowerComponent.DistrictMono.ZoneConnect)
            {
                var countPlayerUnitInZone = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerConnect.GUID
                        && unit.PowerSolidPlayerID == currentPlayerID)
                    .Count();
                
                if (countPlayerUnitInZone - 2 > 0)
                {
                    var needUnit = needCountUnit - (sumCountAddUnit + (countPlayerUnitInZone - 2));
                    if (needUnit < 0)
                    {
                        var countUnit = countPlayerUnitInZone - 2 - Mathf.Abs(needUnit);
                        unitsForAttacks.Add(new ItemValue {Item = towerConnect.GUID, Value = countUnit});
                        sumCountAddUnit += countUnit;
                    }
                    else
                    {
                        var countUnit = countPlayerUnitInZone - 2;
                        unitsForAttacks.Add(new ItemValue {Item = towerConnect.GUID, Value = countUnit});
                        sumCountAddUnit += countUnit;
                    }
                    
                    if (sumCountAddUnit >= needCountUnit)
                        break;
                }
            }

            foreach (var unitForAttackValue in unitsForAttacks)
            {
                for (int i = 0; i < unitForAttackValue.Value; i++)
                {
                    var entityUnit = _dataWorld.Select<UnitMapComponent>()
                        .Without<SelectUnitMapComponent>()
                        .Where<UnitMapComponent>(unit => unit.PowerSolidPlayerID == currentPlayerID
                            && unit.GUIDTower == unitForAttackValue.Item)
                        .SelectFirstEntity();
                    
                    entityUnit.AddComponent(new SelectUnitMapComponent());
                    var unitComponent = entityUnit.GetComponent<UnitMapComponent>();
                    unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();
                }
            }
            
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == _guidCard)
                .SelectFirstEntity();
            entityCard.AddComponent(new AbilityCardMoveUnitComponent {IsAimOn = true, SelectTowerGUID = targetTowerComponent.GUID});
            
            MapMoveUnitsAction.StartMoveUnits?.Invoke();
        }

        public void Destroy()
        {
            AbilityAIAction.MoveUnit -= AbilityMoveUnit;
            AbilityAIAction.CalculatePotentialMoveUnitAttack -= CalculatePotentialMoveUnitAttack;
            AbilityAIAction.CalculatePotentialMoveUnit -= CalculatePotentialMoveUnitOnItsTerritory;
        }
    }
}