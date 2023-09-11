using CyberNet.Core.ActionCard;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
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
            var roundData = _dataWorld.OneData<RoundData>();
            if (roundData.PlayerType != PlayerType.Player)
                return;
            
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);

            if (Physics.Raycast(ray, out var hit, 1500f))
            {
                var towerMono = hit.collider.gameObject.GetComponent<TowerMono>();
                if (towerMono)
                {
                    ClickTower(towerMono, roundData.CurrentPlayerID);
                    return;
                }

                var solidPoint = hit.collider.gameObject.GetComponent<SolidPointMono>();
                if (solidPoint)
                {
                    ClickSolidPoint(solidPoint, roundData.CurrentPlayerID);
                }
            }
        }
        
        private void ClickTower(TowerMono towerMono, int playerID)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();

            if (playerEntity.HasComponent<PlayerNotInstallFirstBaseComponent>())
            {
                SelectFirstBaseAction.SelectBase?.Invoke(towerMono.GUID);
                playerEntity.RemoveComponent<PlayerNotInstallFirstBaseComponent>();
            }
            else
            {
                //TODO: установка шпиона, когда-нибудь
            }
        }
        
        private void ClickSolidPoint(SolidPointMono solidPoint, int currentPlayerID)
        {
            ref var actionData = ref _dataWorld.OneData<ActionCardData>();
            if (actionData.TotalAttack - actionData.SpendAttack == 0)
                return;

            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == currentPlayerID)
                .SelectFirstEntity();
            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            
            var isUnitPoint = _dataWorld.Select<UnitComponent>()
                .Where<UnitComponent>(unit => unit.GUIDPoint == solidPoint.GUID && unit.IndexPoint == solidPoint.Index)
                .TrySelectFirstEntity(out var unitEntity);

            if (isUnitPoint)
            {
                if (actionData.TotalAttack - actionData.SpendAttack < 3)
                    return;
                // Destroy unit
            }
            else
            {
                actionData.SpendAttack++;
                playerComponent.UnitCount--;
                
                var initUnit = new InitUnitStruct {
                    KeyUnit = "blue_unit",
                    SolidPoint  = solidPoint,
                    PlayerControl = PlayerControlEnum.Player,
                    TargetPlayerID = currentPlayerID
                };

                CityAction.InitUnit?.Invoke(initUnit);
                BoardGameUIAction.UpdateStatsPlayersPassportUI?.Invoke();
                BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            }
        }
    }
}