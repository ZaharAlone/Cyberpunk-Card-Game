using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AI;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
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
            Debug.LogError("Ability Set Ice");
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.SetIce?.Invoke(guidCard);
                return;
            }

            StartBazieTower();
            roundData.PauseInteractive = true;
            
            AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.SetIce, 0, false);
            CityAction.ShowWhereZoneToPlayerID?.Invoke(roundData.CurrentPlayerID);
            CityAction.SelectTower += SetIceSelectTower;
        }

        private void StartBazieTower()
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();
            
            var cardComponent = entityCard.GetComponent<CardComponent>();
            var cardPosition = cardComponent.RectTransform.position;
            cardPosition.y += cardComponent.RectTransform.sizeDelta.y / 2;
            BezierCurveNavigationAction.StartBezierCurve?.Invoke(cardPosition, BezierTargetEnum.Tower);
        }
        
        private void SetIceSelectTower(string towerGUID)
        {
            var entityTower = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            var towerComponent = entityTower.GetComponent<TowerComponent>();
            towerComponent.TowerMono.EnableIceZone();

            entityTower.AddComponent(new TowerIceComponent());
            CityAction.SelectTower -= SetIceSelectTower;
            
            FinishWorkAbility();
        }
        
        private void DestroyIce(string guidCard)
        {
            Debug.LogError("Ability Destroy Ice");
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.DestroyIce?.Invoke(guidCard);
                return;
            }

            StartBazieTower();
            roundData.PauseInteractive = true;
            
            var towerWithIceEntities = _dataWorld.Select<TowerComponent>()
                .With<TowerIceComponent>()
                .GetEntities();

            var listID = new List<int>();
            
            foreach (var towerEntity in towerWithIceEntities)
            {
                listID.Add(towerEntity.GetComponent<TowerComponent>().TowerBelongPlayerID);
            }
            
            CityAction.ShowManyZonePlayerInMap?.Invoke(listID);
            
            AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.SetIce, 0, false);
            CityAction.SelectTower += DestroyIceSelectTower;
        }
        
        private void DestroyIceSelectTower(string towerGUID)
        {
            var entityTower = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            
            var towerComponent = entityTower.GetComponent<TowerComponent>();
            towerComponent.TowerMono.DisableIceZone();
            entityTower.RemoveComponent<TowerIceComponent>();
            
            CityAction.SelectTower -= DestroyIceSelectTower;
            FinishWorkAbility();
        }

        private void FinishWorkAbility()
        {
            AbilityCardAction.CurrentAbilityEndPlaying?.Invoke();
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
        }

        public void Destroy() { }
    }
}