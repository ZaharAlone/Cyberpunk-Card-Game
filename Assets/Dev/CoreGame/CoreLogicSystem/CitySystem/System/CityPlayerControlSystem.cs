using System;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using CyberNet.Core.SelectFirstBase;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.GameCamera;
using EcsCore;
using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
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
            RoundData roundData = _dataWorld.OneData<RoundData>();
            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
                return;

            InputData inputData = _dataWorld.OneData<InputData>();
            GameCameraData camera = _dataWorld.OneData<GameCameraData>();
            Ray ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                TowerMono towerMono = hit.collider.gameObject.GetComponent<TowerMono>();
                if (towerMono)
                {
                    ClickTower(towerMono, roundData.CurrentPlayerID);
                    return;
                }

                SquadZoneMono solidPoint = hit.collider.gameObject.GetComponent<SquadZoneMono>();
                if (solidPoint)
                {
                    ClickSolidPoint(solidPoint, roundData.CurrentPlayerID);
                }
            }
        }

        private void ClickTower(TowerMono towerMono, int playerID)
        {
            Entity playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();

            ref ActionCardData actionData = ref _dataWorld.OneData<ActionCardData>();
            var activeAbilityCard = CheckAbilityCard();
            
            if (playerEntity.HasComponent<PlayerNotInstallFirstBaseComponent>())
            {
                SelectFirstBaseAction.SelectBase?.Invoke(towerMono.GUID);
            }
            else if (activeAbilityCard)
            {
                AbilitySelectElementAction.SelectTower?.Invoke(towerMono.GUID);
            }
            else if (actionData.TotalAttack - actionData.SpendAttack > 0)
            {
                AddUnitTower(towerMono, playerID);
            }
        }

        private bool CheckAbilityCard()
        {
            var isElementAbilityAction = _dataWorld.Select<AbilitySelectElementComponent>().Count();
            return isElementAbilityAction > 0;
        }

        private void AddUnitTower(TowerMono towerMono, int playerID)
        {
            Entity playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();

            ref ActionCardData actionData = ref _dataWorld.OneData<ActionCardData>();
            ref PlayerComponent playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref PlayerViewComponent playerVisualComponent = ref playerEntity.GetComponent<PlayerViewComponent>();

            actionData.SpendAttack++;
            playerComponent.UnitCount--;

            Entity towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerMono.GUID)
                .SelectFirstEntity();
            ref TowerComponent towerComponent = ref towerEntity.GetComponent<TowerComponent>();

            int targetSquadZone = 0;
            foreach (SquadZoneMono squadZone in towerComponent.SquadZonesMono)
            {
                bool isClose = _dataWorld.Select<SquadMapComponent>()
                    .Where<SquadMapComponent>(unit => unit.GUIDPoint == towerMono.GUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID != playerID)
                    .TrySelectFirstEntity(out Entity t);

                if (isClose)
                    targetSquadZone = squadZone.Index + 1;
                else
                {
                    targetSquadZone = squadZone.Index;
                    break;
                }
            }

            InitUnitStruct initUnit = new InitUnitStruct {
                KeyUnit = playerVisualComponent.KeyCityVisual, SquadZone = towerMono.SquadZonesMono[targetSquadZone], PlayerControl = PlayerControlEnum.Player, TargetPlayerID = playerID,
            };

            CityAction.InitUnit?.Invoke(initUnit);
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            BoardGameUIAction.UpdateStatsMainPlayersPassportUI?.Invoke();
            BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
        }

        private void ClickSolidPoint(SquadZoneMono squadZone, int currentPlayerID)
        {
            ref ActionCardData actionData = ref _dataWorld.OneData<ActionCardData>();
            if (actionData.TotalAttack - actionData.SpendAttack == 0)
                return;

            ref BoardGameRuleSettings rulesGame = ref _dataWorld.OneData<BoardGameData>().BoardGameRule;
            Entity playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            ref PlayerComponent playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref PlayerViewComponent playerVisualComponent = ref playerEntity.GetComponent<PlayerViewComponent>();

            bool isUnitPoint = _dataWorld.Select<SquadMapComponent>()
                .Where<SquadMapComponent>(unit => unit.GUIDPoint == squadZone.GUID && unit.IndexPoint == squadZone.Index)
                .TrySelectFirstEntity(out Entity unitEntity);

            /*
            if (isUnitPoint)
            {
                if (actionData.TotalAttack - actionData.SpendAttack < rulesGame.PriceKillSquad)
                    return;

                actionData.SpendAttack += rulesGame.PriceKillSquad;

                ref var unitComponent = ref unitEntity.GetComponent<SquadMapComponent>();
                if (unitComponent.PowerSolidPlayerID == currentPlayerID)
                    return;

                playerComponent.VictoryPoint += rulesGame.RewardKillSquad;
                CityAction.AttackSolidPoint?.Invoke(unitComponent.GUIDPoint, unitComponent.IndexPoint);
                CityAction.UpdatePresencePlayerInCity?.Invoke();
                BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            }
            else
            {
                actionData.SpendAttack++;
                playerComponent.UnitCount--;

                var initUnit = new InitUnitStruct {
                    KeyUnit = playerVisualComponent.KeyCityVisual,
                    SquadZone  = squadZone,
                    PlayerControl = PlayerControlEnum.Player,
                    TargetPlayerID = currentPlayerID
                };

                CityAction.InitUnit?.Invoke(initUnit);
                CityAction.UpdatePresencePlayerInCity?.Invoke();
                BoardGameUIAction.UpdateStatsMainPlayersPassportUI?.Invoke();
                BoardGameUIAction.UpdateStatsPlayersCurrency?.Invoke();
            }*/
        }
    }
}