using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Global;
using CyberNet.Global.GameCamera;
using Input;

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
        }
        
        public void CheckClick()
        {
            Debug.LogError("CheckClick");
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.PlayerType != PlayerType.Player)
                return;
            Debug.LogError("Input");
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);

            if (Physics.Raycast(ray, out var hit, 1500f))
            {
                Debug.LogError("Raycast");
                var towerMono = hit.collider.gameObject.GetComponent<TowerMono>();
                if (towerMono)
                    ClickTower(towerMono, roundData.CurrentPlayerID);
            }
        }
        
        private void ClickTower(TowerMono towerMono, int playerID)
        {
            Debug.LogError($"Click Tower player ID {playerID}");
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();

            if (playerEntity.HasComponent<PlayerNotInstallFirstBaseComponent>())
            {
                Debug.LogError("Init Tower");
                SelectFirstBaseAction.SelectBase?.Invoke(towerMono.GUID);
            }
            else
            {
                //TODO: установка шпиона, когда-нибудь
            }
        }
        
        private void ClickSolidPoint(SolidPointMono solidPoint)
        {
            
        }
    }
}