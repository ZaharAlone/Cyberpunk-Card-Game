using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Core.UI.TaskPlayerPopup;
using CyberNet.Global;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.SelectFirstBase
{
    [EcsSystem(typeof(CoreModule))]
    public class SelectFirstBaseSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectFirstBaseAction.CheckInstallFirstBase += CheckPlayerSelectFirstBase;
            SelectFirstBaseAction.SelectBase += SelectBase;
        }

        private bool CheckPlayerSelectFirstBase()
        {
            var isNotInstallFirstBase = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .With<PlayerNotInstallFirstBaseComponent>()
                .TrySelectFirstEntity(out var playerEntity);

            if (!isNotInstallFirstBase)
                return true;
            
            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();

            if (isNotInstallFirstBase && playerComponent.playerOrAI == PlayerOrAI.Player)
            {
                SelectFirstBase();
            }

            return !isNotInstallFirstBase;
        }

        private void SelectFirstBase()
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = true;
            ref var tradeRowUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            tradeRowUI.ForceFullHidePanel();
            
            TaskPlayerPopupAction.OpenPopupSelectFirstBase?.Invoke();
            CityAction.ShowFirstBaseTower?.Invoke();
        }
        
        private void SelectBase(string towerGUID)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            
            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref var playerVisualComponent = ref playerEntity.GetComponent<PlayerViewComponent>();
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();

            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();

            var targetSquadZone = 0;
            foreach (var squadZone in towerComponent.SquadZonesMono)
            {
                var isClose = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.IndexPoint == squadZone.Index)
                    .TrySelectFirstEntity(out var t);

                if (isClose)
                    targetSquadZone = squadZone.Index+1;
                else
                {
                    targetSquadZone = squadZone.Index;
                    break;
                }
            }

            var initUnit = new InitUnitStruct {
                KeyUnit = playerVisualComponent.KeyCityVisual,
                UnitZone  = towerComponent.SquadZonesMono[targetSquadZone],
                PlayerControl = PlayerControlEntity.PlayerControl,
                TargetPlayerID = playerComponent.PlayerID
            };

            //ADD 2 Unit
            var gameRuleInitUnit = _dataWorld.OneData<BoardGameData>().BoardGameRule.StartInitCountSquad;
            for (int i = 0; i < gameRuleInitUnit; i++)
            {
                CityAction.InitUnit?.Invoke(initUnit);
            }

            playerComponent.UnitCount -= gameRuleInitUnit;
            playerComponent.CurrentCountControlTerritory++;
            towerComponent.PlayerControlEntity = PlayerControlEntity.PlayerControl;
            towerComponent.TowerBelongPlayerID = playerComponent.PlayerID;
            towerEntity.RemoveComponent<FirstBasePlayerComponent>();
            playerEntity.RemoveComponent<PlayerNotInstallFirstBaseComponent>();
            
            ref var tradeRowUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            tradeRowUI.ShowPanelBaseViewAnimations();

            TaskPlayerPopupAction.HidePopup?.Invoke();
            CityAction.HideFirstBaseTower?.Invoke();
            RoundAction.StartTurn?.Invoke();
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();
            CityAction.UpdatePlayerViewCity?.Invoke();
        }

        public void Destroy()
        {
            SelectFirstBaseAction.CheckInstallFirstBase -= CheckPlayerSelectFirstBase;
            SelectFirstBaseAction.SelectBase -= SelectBase;
        }
    }
}