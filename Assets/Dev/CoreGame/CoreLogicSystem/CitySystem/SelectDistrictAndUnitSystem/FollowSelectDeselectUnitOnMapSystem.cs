using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Map.InteractiveElement.Support;
using CyberNet.Global.Sound;

namespace CyberNet.Core.Map.InteractiveElement
{
    [EcsSystem(typeof(CoreModule))]
    public class FollowSelectDeselectUnitOnMapSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            FollowSelectInteractiveMapAction.StartFollowSelectUnit += StartFollowSelectUnit;
            FollowSelectInteractiveMapAction.EndFollowSelectUnit += EndFollowUnit;
        }

        private void StartFollowSelectUnit(TargetDistrictAndPlayerIDDTO targetDistrictConfig)
        {
            CityAction.ShowWherePlayerCanMoveFrom?.Invoke(targetDistrictConfig.GUIDDistrict);
            CityAction.ActivationsColliderUnitsInTower?.Invoke(targetDistrictConfig.GUIDDistrict, targetDistrictConfig.TargetPlayerID);
            CityAction.SelectUnit += ClickOnUnit;

            _dataWorld.NewEntity().AddComponent(new FollowClickUnitComponent());
        }
        
        private void ClickOnUnit(string unitGUID)
        {
            var unitEntity = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDUnit == unitGUID)
                .SelectFirstEntity();

            var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
            
            if (unitEntity.HasComponent<SelectUnitMapComponent>())
            {
                unitEntity.RemoveComponent<SelectUnitMapComponent>();
                unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.DeselectUnitInMap);
            }
            else
            {
                unitEntity.AddComponent(new SelectUnitMapComponent());
                unitComponent.IconsUnitInMapMono.OnSelectUnitEffect();
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.SelectUnitInMap);
            }

            FollowSelectInteractiveMapAction.UpdateSelectUnit?.Invoke();
        }

        private void EndFollowUnit()
        {
            var followUnitEntities = _dataWorld.Select<FollowClickUnitComponent>().GetEntities();

            foreach (var followUnitEntity in followUnitEntities)
                followUnitEntity.Destroy();
        }
        
        public void Destroy()
        {
            FollowSelectInteractiveMapAction.StartFollowSelectUnit -= StartFollowSelectUnit;
            FollowSelectInteractiveMapAction.EndFollowSelectUnit -= EndFollowUnit;
        }
    }
}