using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.City;
using CyberNet.Tools;
using Object = UnityEngine.Object;

namespace CyberNet.Core
{
    /// <summary>
    /// Система первично настраивает город, создает карту и расставляет нейтральных юнитов
    /// </summary>
    [EcsSystem(typeof(CoreModule))]
    public class CitySetupSystem : IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

public void Init()
        {
            SetupBoard();
            SetupInteractiveElement();
        }

        //Создаем поле
        private void SetupBoard()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var cityMono = Object.Instantiate(boardGameData.BoardGameConfig.CityMono);
            
            _dataWorld.CreateOneData(new CityData {
                CityGO = cityMono.gameObject,
                CityMono = cityMono,
                SolidConteiner = cityMono.SolidContainer
            });
            GlobalCoreAction.FinishInitGameResource?.Invoke();
        }

        //Инициализируем все интерактивные объекты на карте
        private void SetupInteractiveElement()
        {
            //TODO Пересмотреть Добавить визуал к кускам карты
            var cityData = _dataWorld.OneData<CityData>();
            
            foreach (var tower in cityData.CityMono.Towers)
            {
                var entity = _dataWorld.NewEntity();
                tower.DeactivateCollider();
                tower.CloseInteractiveZoneVisualEffect();
                
                var towerComponent = new TowerComponent 
                {
                    GUID = tower.GUID,
                    Key = tower.Key,
                    TowerMono = tower,
                    TowerGO = tower.gameObject,
                    SquadZonesMono = tower.SquadZonesMono,
                    VisualEffectZone = tower.VisualEffectZone,
                    PlayerControlEntity = PlayerControlEntity.None,
                };
                
                foreach (var squadZone in tower.SquadZonesMono)
                {
                    var countUnit =  InitStartUnitReturnCount(squadZone);
                    if (countUnit > 0)
                    {
                        towerComponent.PlayerControlEntity = PlayerControlEntity.NeutralUnits;
                    }
                }

                entity.AddComponent(towerComponent);
                if (tower.IsFirstBasePlayer)
                    entity.AddComponent(new FirstBasePlayerComponent());
            }
            
            CityAction.UpdatePlayerViewCity?.Invoke();
        }

        private int InitStartUnitReturnCount(UnitZoneMono UnitZone)
        {
            var countInit = 0;
            var countNeutralUnitInTower = _dataWorld.OneData<BoardGameData>().BoardGameRule.CountNeutralUnitInTower;
            if (UnitZone.StartIsNeutralSolid)
            {
                for (int i = 0; i < countNeutralUnitInTower; i++)
                {
                    var neutralUnit = new InitUnitStruct 
                    {
                        KeyUnit = "neutral_unit",
                        UnitZone = UnitZone,
                        PlayerControl = PlayerControlEntity.NeutralUnits,
                        TargetPlayerID = -1
                    };
                
                    CityAction.InitUnit?.Invoke(neutralUnit);
                    countInit++;   
                }
            }

            return countInit;
        }

        public void Destroy()
        {
            ref var resourceTable = ref _dataWorld.OneData<CityData>();
            Object.Destroy(resourceTable.CityGO);

            _dataWorld.RemoveOneData<CityData>();
        }
    }
}