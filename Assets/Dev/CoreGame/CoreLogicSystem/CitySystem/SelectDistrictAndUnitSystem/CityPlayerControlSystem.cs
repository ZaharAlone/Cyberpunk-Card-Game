using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Global;
using CyberNet.Global.GameCamera;
using EcsCore;
using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.Map.InteractiveElement
{
    [EcsSystem(typeof(CoreModule))]
    public class CityPlayerControlSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            if (_dataWorld.OneData<InputData>().Click)
            {
                var isFollowSelectDistrict = _dataWorld.Select<FollowClickDistrictComponent>().Count() > 0;
                var isFollowSelectUnits = _dataWorld.Select<FollowClickUnitComponent>().Count() > 0;
                
                if (isFollowSelectDistrict)
                    CheckClickDistrict();
                if (isFollowSelectUnits)
                    CheckClickUnit();
            }
        }
        
        private void CheckClickDistrict()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.playerOrAI != PlayerOrAI.Player)
                return;

            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var towerMono = hit.collider.gameObject.GetComponent<DistrictMono>();
                if (towerMono)
                {
                    if (towerMono.IsInteractiveTower)
                        ClickTower(towerMono);
                }
            }
        }
        
        private void ClickTower(DistrictMono DistrictMono)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            
            if (playerEntity.HasComponent<PlayerNotInstallFirstBaseComponent>())
                SelectFirstBaseAction.SelectBase?.Invoke(DistrictMono.GUID);
            else
                CityAction.SelectDistrict?.Invoke(DistrictMono.GUID);
        }

        private void CheckClickUnit()
        {
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.playerOrAI != PlayerOrAI.Player)
                return;

            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var unitPoint = hit.collider.gameObject.GetComponent<IconsContainerUnitInMapMono>();
                if (unitPoint)
                    ClickSolidPoint(unitPoint, roundData.CurrentPlayerID);
            }
        }
        
        private void ClickSolidPoint(IconsContainerUnitInMapMono unitPoint, int currentPlayerID)
        {
            var unitGuid = unitPoint.GetGUID();
            
            var isUnitPoint = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDUnit == unitGuid)
                .TrySelectFirstEntity(out var unitEntity);
            
            if (isUnitPoint)
            {
                ref var unitComponent = ref unitEntity.GetComponent<UnitMapComponent>();
                if (unitComponent.PowerSolidPlayerID == currentPlayerID)
                    CityAction.SelectUnit?.Invoke(unitGuid);
            }
        }
    }
}