using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AI;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.Map;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityIceSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.SetIce += SetIce;
            AbilityCardAction.DestroyIce += DestroyIce;
        }
        
        private void SetIce(string guidCard)
        {
            var roundData = _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.SetIce?.Invoke(guidCard);
                return;
            }

            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Tower);
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(AbilityType.SetIce, 0, false);
            CityAction.ShowWhereZoneToPlayerID?.Invoke(roundData.CurrentPlayerID);
            CityAction.SelectDistrict += SetIceSelectTower;
        }
        
        private void SetIceSelectTower(string towerGUID)
        {
            var entityTower = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            var towerComponent = entityTower.GetComponent<DistrictComponent>();
            towerComponent.DistrictMono.EnableIceZone();

            entityTower.AddComponent(new DistrictIceComponent());
            CityAction.SelectDistrict -= SetIceSelectTower;
            
            FinishWorkAbility();
        }
        
        private void DestroyIce(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.DestroyIce?.Invoke(guidCard);
                return;
            }

            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Tower);

            var towerWithIceEntities = _dataWorld.Select<DistrictComponent>()
                .With<DistrictIceComponent>()
                .GetEntities();

            var listID = new List<int>();
            
            foreach (var towerEntity in towerWithIceEntities)
            {
                listID.Add(towerEntity.GetComponent<DistrictComponent>().DistrictBelongPlayerID);
            }
            
            CityAction.ShowManyZonePlayerInMap?.Invoke(listID);
            
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(AbilityType.SetIce, 0, false);
            CityAction.SelectDistrict += DestroyIceSelectTower;
        }
        
        private void DestroyIceSelectTower(string towerGUID)
        {
            var entityTower = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            
            var towerComponent = entityTower.GetComponent<DistrictComponent>();
            towerComponent.DistrictMono.DisableIceZone();
            entityTower.RemoveComponent<DistrictIceComponent>();
            
            CityAction.SelectDistrict -= DestroyIceSelectTower;
            FinishWorkAbility();
        }

        private void FinishWorkAbility()
        {
            AbilityCardAction.CurrentAbilityEndPlaying?.Invoke();
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
        }

        public void Destroy()
        {
            AbilityCardAction.SetIce -= SetIce;
            AbilityCardAction.DestroyIce -= DestroyIce;
            
            CityAction.SelectDistrict -= SetIceSelectTower;
            CityAction.SelectDistrict -= DestroyIceSelectTower;
        }
    }
}