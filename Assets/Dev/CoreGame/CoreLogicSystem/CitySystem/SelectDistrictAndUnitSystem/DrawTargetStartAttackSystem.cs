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
    }
}