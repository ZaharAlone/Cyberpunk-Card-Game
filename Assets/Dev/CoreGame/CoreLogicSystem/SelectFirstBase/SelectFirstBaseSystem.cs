using CyberNet.Core.Map;
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
            SelectFirstBaseAction.SelectBase += SelectedBase;
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

            if (playerComponent.playerOrAI == PlayerOrAI.Player)
            {
                SetupInterfaceSelectFirstBase();
            }

            return false;
        }

        private void SetupInterfaceSelectFirstBase()
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = true;
            ref var tradeRowUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            tradeRowUI.ForceFullHidePanel();
            
            TaskPlayerPopupAction.OpenPopupSelectFirstBase?.Invoke();
            CityAction.ShowFirstBaseTower?.Invoke();
        }
        
        private void SelectedBase(string towerGUID)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            
            ref var playerComponent = ref playerEntity.GetComponent<PlayerComponent>();
            ref var playerVisualComponent = ref playerEntity.GetComponent<PlayerViewComponent>();
            
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();

            ref var towerComponent = ref towerEntity.GetComponent<DistrictComponent>();

            var targetSquadZone = SelectTargetZone(towerGUID);

            var initUnit = new InitUnitStruct {
                KeyUnit = playerVisualComponent.KeyCityVisual,
                UnitZone  = towerComponent.SquadZonesMono[targetSquadZone],
                PlayerControl = PlayerControlEntity.PlayerControl,
                TargetPlayerID = playerComponent.PlayerID
            };
            
            var gameRuleInitUnit = _dataWorld.OneData<BoardGameData>().BoardGameRule.StartInitCountSquad;
            for (int i = 0; i < gameRuleInitUnit; i++)
            {
                CityAction.InitUnit?.Invoke(initUnit);
            }

            playerComponent.UnitCount -= gameRuleInitUnit;
            playerComponent.VictoryPoint++;
            towerComponent.PlayerControlEntity = PlayerControlEntity.PlayerControl;
            towerComponent.DistrictBelongPlayerID = playerComponent.PlayerID;
            
            towerEntity.RemoveComponent<FirstBasePlayerComponent>();
            playerEntity.RemoveComponent<PlayerNotInstallFirstBaseComponent>();
            
            var tradeRowUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.TraderowMono;
            tradeRowUI.TradeRowToMiniPanelAnimations();

            TaskPlayerPopupAction.ClosePopup?.Invoke();
            CityAction.HideFirstBaseTower?.Invoke();
            RoundAction.StartTurn?.Invoke();
            
            CityAction.UpdatePresencePlayerInCity?.Invoke();
        }

        private int SelectTargetZone(string towerGUID)
        {
            var towerEntity = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();

            var towerComponent = towerEntity.GetComponent<DistrictComponent>();
            
            var targetSquadZone = 0;
            foreach (var squadZone in towerComponent.SquadZonesMono)
            {
                var isClose = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.IndexPoint == squadZone.Index)
                    .Count() > 0;

                if (isClose)
                    targetSquadZone = squadZone.Index+1;
                else
                {
                    targetSquadZone = squadZone.Index;
                    break;
                }
            }
            
            return targetSquadZone;
        }

        public void Destroy()
        {
            SelectFirstBaseAction.CheckInstallFirstBase -= CheckPlayerSelectFirstBase;
            SelectFirstBaseAction.SelectBase -= SelectedBase;
        }
    }
}