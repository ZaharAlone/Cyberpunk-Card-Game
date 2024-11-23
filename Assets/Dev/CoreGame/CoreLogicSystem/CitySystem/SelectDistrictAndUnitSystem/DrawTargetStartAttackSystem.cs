using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.MapMoveUnit;
using CyberNet.Global.Cursor;
using CyberNet.Global.GameCamera;
using Input;

namespace CyberNet.Core.Map.InteractiveElement
{
    [EcsSystem(typeof(CoreModule))]
    public class DrawTargetStartAttackSystem : IRunSystem
    {
        private DataWorld _dataWorld;

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
            ref var moveUnitComponent = ref entityMoveCard.GetComponent<MoveUnitComponent>();
                
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);
            var isCurrentTowerSelect = false;
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var towerMono = hit.collider.gameObject.GetComponent<DistrictMono>();
                if (towerMono)
                {
                    if (towerMono.GUID == moveUnitComponent.TargetToMoveDistrictGUID && towerMono.IsInteractiveTower)
                    {
                        isCurrentTowerSelect = true;
                        if (!moveUnitComponent.IsAimOn)
                        {
                            moveUnitComponent.IsAimOn = true;
                            CustomCursorAction.OnAimCursor?.Invoke();
                            EnableFollowClickDistrict();
                        }
                    }
                }
            }
            
            if (!isCurrentTowerSelect && moveUnitComponent.IsAimOn)
            {
                CustomCursorAction.OnBaseCursor?.Invoke();
                moveUnitComponent.IsAimOn = false;
                DisableFollowClickDistrict();
            }
        }
        
        private void EnableFollowClickDistrict()
        {
            var isFollowDistrict = _dataWorld.Select<MoveUnitComponent>()
                .With<FollowClickDistrictComponent>()
                .Count() > 0;
            if (isFollowDistrict)
                return;

            var moveUnitEntity = _dataWorld.Select<MoveUnitComponent>().SelectFirstEntity();
            moveUnitEntity.AddComponent(new FollowClickDistrictComponent());
        }

        private void DisableFollowClickDistrict()
        {
            var isFollowClickDistrictQuery = _dataWorld.Select<FollowClickDistrictComponent>();

            if (isFollowClickDistrictQuery.Count() > 0)
            {
                foreach (var followClickEntity in isFollowClickDistrictQuery.GetEntities())
                {
                    followClickEntity.RemoveComponent<FollowClickDistrictComponent>();
                }
            }
        }
    }
}