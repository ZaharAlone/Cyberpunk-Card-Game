using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI.PopupDistrictInfo;
using CyberNet.Global;
using CyberNet.Global.GameCamera;
using EcsCore;
using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
namespace CyberNet.Core.City
{
    [EcsSystem(typeof(CoreModule))]
    public class CityPlayerControlSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            if (_dataWorld.OneData<InputData>().Click)
                CheckClick();
            
            if (_dataWorld.OneData<RoundData>().CurrentRoundState == RoundState.Map)
                ReadMouseInput();    
        }
        
        public void CheckClick()
        {
            RoundData roundData = _dataWorld.OneData<RoundData>();
            if (roundData.playerOrAI != PlayerOrAI.Player)
                return;

            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var towerMono = hit.collider.gameObject.GetComponent<TowerMono>();
                if (towerMono)
                {
                    if (towerMono.IsInteractiveTower)
                    {
                        ClickTower(towerMono);
                        return;   
                    }
                }

                var unitPoint = hit.collider.gameObject.GetComponent<IconsUnitInMapMono>();
                if (unitPoint)
                {
                    ClickSolidPoint(unitPoint, roundData.CurrentPlayerID);
                }
            }
        }

        private void ClickTower(TowerMono towerMono)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var activeAbilityCard = CheckAbilityCard();
            
            if (playerEntity.HasComponent<PlayerNotInstallFirstBaseComponent>())
            {
                SelectFirstBaseAction.SelectBase?.Invoke(towerMono.GUID);
            }
            else if (activeAbilityCard)
            {
                CityAction.SelectTower?.Invoke(towerMono.GUID);
            }
        }

        private bool CheckAbilityCard()
        {
            var isElementAbilityAction = _dataWorld.Select<AbilitySelectElementComponent>().Count();
            return isElementAbilityAction > 0;
        }

        private void ClickSolidPoint(IconsUnitInMapMono unitPoint, int currentPlayerID)
        {
            var unitGuid = unitPoint.GetGUID();
            
            var isUnitPoint = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDUnit == unitGuid)
                .TrySelectFirstEntity(out var unitEntity);
            
            if (isUnitPoint)
            {
                ref var unitComponent = ref unitEntity.GetComponent<UnitMapComponent>();
                if (unitComponent.PowerSolidPlayerID == currentPlayerID)
                {
                    CityAction.SelectUnit?.Invoke(unitGuid);
                }
            }
        }
        
        private void ReadMouseInput()
        {
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);

            var isRaycastDistrict = false;
            
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = inputData.MousePosition
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var towerMono = hit.collider.gameObject.GetComponent<TowerMono>();
                if (towerMono && results.Count == 0)
                {
                    isRaycastDistrict = true;
                    PopupDistrictInfoAction.OpenPopup?.Invoke(towerMono.GUID);
                }
            }
            
            if (!isRaycastDistrict)
            {
                PopupDistrictInfoAction.ClosePopup?.Invoke();
            }
        }
    }
}