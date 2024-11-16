using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.Map;
using CyberNet.Core.UI.PopupDistrictInfo;
using CyberNet.Global.GameCamera;
using Input;
using UnityEngine.EventSystems;

namespace CyberNet.Core.Map.DistrictPopupInfo
{
    [EcsSystem(typeof(CoreModule))]
    public class CityDistrictPopupInfoSystem : IRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            if (_dataWorld.OneData<RoundData>().CurrentGameStateMapVSArena == GameStateMapVSArena.Map)
                ReadMouseInput();  
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
                var towerMono = hit.collider.gameObject.GetComponent<DistrictMono>();
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