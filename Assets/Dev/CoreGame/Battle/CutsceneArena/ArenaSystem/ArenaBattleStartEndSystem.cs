using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.Map;
using CyberNet.Core.UI;
using CyberNet.Core.UI.PopupDistrictInfo;
using CyberNet.Global;
using CyberNet.Global.BlackScreen;
using CyberNet.Global.GameCamera;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Battle.CutsceneArena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaBattleStartEndSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            BattleCutsceneAction.StartCutscene += StartArenaBattle;
        }
        
        private void StartArenaBattle()
        {
            BlackScreenUIAction.AnimationsShowScreen?.Invoke();
            
            ControlViewGameUI(true);
            EnableVisualArena();

            BlackScreenUIAction.AnimationsHideScreen?.Invoke();
            WaitEndBattle();
            //_dataWorld.InitModule<ArenaModule>(true);
        }
        
        private void EnableVisualArena()
        {
            var arenaData = _dataWorld.OneData<ArenaData>();
            var cameraData = _dataWorld.OneData<GameCameraData>();
            var cityData = _dataWorld.OneData<CityData>();
            
            arenaData.ArenaMono.EnableArena();
            arenaData.ArenaMono.EnableCamera();
            cameraData.CameraGO.SetActive(false);
            cityData.CityMono.OffCityLight();
        }
        
        private void ControlViewGameUI(bool isOpenArena)
        {
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            
            if (isOpenArena)
            {
                boardGameUI.CoreHudUIMono.HideCurrentPlayer();
                boardGameUI.CoreHudUIMono.HideEnemyPassport();
                boardGameUI.CoreHudUIMono.HideButtons();
                boardGameUI.CoreHudUIMono.SetEnableMainPlayerCurrentRound(false);
                boardGameUI.TraderowMono.DisableTradeRow();
                boardGameUI.ActionButtonMono.DisableButton();
            }
            else
            {
                boardGameUI.CoreHudUIMono.ShowCurrentPlayer();
                boardGameUI.CoreHudUIMono.ShowEnemyPassport();
                boardGameUI.CoreHudUIMono.ShowButtons();
                boardGameUI.TraderowMono.EnableTradeRow();
                boardGameUI.CoreHudUIMono.SetEnableMainPlayerCurrentRound(true);
                boardGameUI.ActionButtonMono.EnableButton();
            }
            
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var cardInPlayerHandEntities = _dataWorld.Select<CardComponent>()
                .With<CardHandComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .GetEntities();

            foreach (var entityDiscardCard in cardInPlayerHandEntities)
            {
                var cardMono = entityDiscardCard.GetComponent<CardComponent>().CardMono;
                if (isOpenArena)
                    cardMono.HideCard();
                else
                    cardMono.ShowCard();
            }
        }
        
        //TODO заглушка для первой итерации
        private void WaitEndBattle()
        {
            var waitEndBattle = new TimeComponent {
                Time = 2f, Action = () => EndBattleArena()
            };

            _dataWorld.NewEntity().AddComponent(waitEndBattle);
        }
        
        private void EndBattleArena()
        {
            BattleAction.CalculateResultBattle?.Invoke();
            
            SwitchingVisualsArenaEndBattle();
            ControlViewGameUI(false);
//            _dataWorld.DestroyModule<ArenaModule>();
            
            BattleAction.KillUnitInMapView?.Invoke();
        }

        private void SwitchingVisualsArenaEndBattle()
        {
            BlackScreenUIAction.AnimationsShowScreen?.Invoke();
            var arenaData = _dataWorld.OneData<ArenaData>();
            var cameraData = _dataWorld.OneData<GameCameraData>();
            var cityData = _dataWorld.OneData<CityData>();

            arenaData.ArenaMono.DisableArena();
            arenaData.ArenaMono.DisableCamera();
            cameraData.CameraGO.SetActive(true);
            cityData.CityMono.OnCityLight();

            //ClearArenaUnitsOnArena();
            
            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = false;
            roundData.CurrentGameStateMapVSArena = GameStateMapVSArena.Map;
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
            BlackScreenUIAction.AnimationsHideScreen?.Invoke();
        }

        private void ClearArenaUnitsOnArena()
        {
            var playerInArenaEntities = _dataWorld.Select<PlayerInBattleComponent>().GetEntities();
            
            foreach (var playerEntity in playerInArenaEntities)
            {
                playerEntity.Destroy();
            }

            var unitInArenaEntities = _dataWorld.Select<ArenaUnitComponent>().GetEntities();

            foreach (var unitEntity in unitInArenaEntities)
            {
                var unitComponent = unitEntity.GetComponent<ArenaUnitComponent>();
                Object.Destroy(unitComponent.UnitGO);
                unitEntity.RemoveComponent<ArenaUnitComponent>();
               // unitEntity.RemoveComponent<ArenaUnitCurrentComponent>();
                unitEntity.RemoveComponent<UnitInBattleArenaComponent>();
            }

            /*
            var unitSelectForAttack = _dataWorld.Select<ArenaSelectUnitForAttackComponent>()
                .GetEntities();
            foreach (var unitEntity in unitSelectForAttack)
            {
                unitEntity.RemoveComponent<ArenaSelectUnitForAttackComponent>();
            }*/
        }

        public void Destroy()
        {
            if (_dataWorld.IsModuleActive<ArenaModule>())
                _dataWorld.DestroyModule<ArenaModule>();

            BattleCutsceneAction.StartCutscene -= StartArenaBattle;

            var arenaMono = _dataWorld.OneData<ArenaData>().ArenaMono;

            if (arenaMono != null)
                Object.Destroy(_dataWorld.OneData<ArenaData>().ArenaMono.gameObject);

            _dataWorld.RemoveOneData<ArenaData>();

            var arenaUnitEntities = _dataWorld.Select<ArenaUnitComponent>().GetEntities();
            foreach (var entity in arenaUnitEntities)
            {
                entity.Destroy();
            }

            //TODO Поправить Арена
            /*
            var playerInArenaEntities = _dataWorld.Select<PlayerArenaInBattleComponent>().GetEntities();
            foreach (var entity in playerInArenaEntities)
            {
                entity.Destroy();
            }*/
        }
    }
}