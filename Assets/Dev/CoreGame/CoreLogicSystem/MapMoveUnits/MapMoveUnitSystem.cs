using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Global.Cursor;
using CyberNet.Global.GameCamera;
using CyberNet.Global.Sound;
using Input;

namespace CyberNet.Core.MapMoveUnit
{
    [EcsSystem(typeof(CoreModule))]
    public class MapMoveUnitSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            MapMoveUnitsAction.PlayerStartSelectingDistrictFromCard += PlayerStartSelectingTargetDistrictFromCard;
        }

        public void Run()
        {
            var gameStateIsArena = _dataWorld.OneData<RoundData>().CurrentGameStateMapVSArena == GameStateMapVSArena.Arena;
            var isNotSelectTowerToMoveUnit = _dataWorld.Select<MoveUnitSelectTowerComponent>().Count() == 0;
            
            if (isNotSelectTowerToMoveUnit || gameStateIsArena)
                return;

            DrawTargetStartAttack();
        }

        private void DrawTargetStartAttack()
        {
            var countSelectUnit = _dataWorld.Select<SelectUnitMapComponent>()
                .Count();
            
            if (countSelectUnit == 0)
                return;
            
            var entityMoveCard = _dataWorld.Select<MoveUnitComponent>().SelectFirstEntity();
            ref var abilityCardMoveUnitComponent = ref entityMoveCard.GetComponent<MoveUnitComponent>();
                
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);
            var isCurrentTowerSelect = false;
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var towerMono = hit.collider.gameObject.GetComponent<DistrictMono>();
                if (towerMono)
                {
                    if (towerMono.GUID == abilityCardMoveUnitComponent.SelectDistrictGUID && towerMono.IsInteractiveTower)
                    {
                        isCurrentTowerSelect = true;
                        if (!abilityCardMoveUnitComponent.IsAimOn)
                        {
                            abilityCardMoveUnitComponent.IsAimOn = true;
                            CustomCursorAction.OnAimCursor?.Invoke();
                            CityAction.SelectDistrict += SelectDistrictToMove;
                        }
                    }
                }
            }
            
            if (!isCurrentTowerSelect && abilityCardMoveUnitComponent.IsAimOn)
            {
                CustomCursorAction.OnBaseCursor?.Invoke();
                abilityCardMoveUnitComponent.IsAimOn = false;
                CityAction.SelectDistrict -= SelectDistrictToMove;
            }
        }

        private void SelectDistrictToMove(string districtGUID)
        {
            var entityMoveCard = _dataWorld.Select<MoveUnitComponent>().SelectFirstEntity();
            ref var abilityCardMoveUnitComponent = ref entityMoveCard.GetComponent<MoveUnitComponent>();
            
            if (districtGUID != abilityCardMoveUnitComponent.SelectDistrictGUID)
                return;
            
            CityAction.SelectUnit -= ClickOnUnit;
            CityAction.SelectDistrict -= SelectDistrictToMove;
            ConfirmMove();
        }

        private void ConfirmMove()
        {
            MapMoveUnitsAction.StartMoveUnits?.Invoke();
            CityAction.DeactivationsColliderAllUnits?.Invoke();
            EndPlayingCard();
        }

        private void EndPlayingCard()
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<MoveUnitComponent>()
                .SelectFirstEntity();

            var cardComponent = entityCard.GetComponent<CardComponent>();
            entityCard.RemoveComponent<MoveUnitComponent>();
            entityCard.RemoveComponent<MoveUnitSelectTowerComponent>();
            AbilityCardAction.CompletePlayingAbilityCard?.Invoke(cardComponent.GUID);

            CityAction.UpdateCanInteractiveMap?.Invoke();
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            CustomCursorAction.OnBaseCursor?.Invoke();
        }
        
        public void Destroy()
        {
            MapMoveUnitsAction.PlayerStartSelectingDistrictFromCard -= PlayerStartSelectingTargetDistrictFromCard;
            CityAction.SelectDistrict -= SelectTower;
        }
    }
}