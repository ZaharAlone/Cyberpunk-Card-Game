using CyberNet.Core.AI;
using CyberNet.Core.Battle.CutsceneArena;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.Map;
using CyberNet.Core.UI;
using CyberNet.Core.UI.PopupDistrictInfo;
using CyberNet.Global;
using CyberNet.Global.GameCamera;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaBattleStartEndSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        private readonly Vector3 _arenaPosition = new Vector3(-150f, 0f, 200f);
        
        public void PreInit()
        {
            CutsceneArenaAction.StartCutscene += StartArenaBattle;
        }
        
        public void Init()
        {
            CreateArenaStartCoreGame();
        }

        private void CreateArenaStartCoreGame()
        {
            var arenaMonoPrefab = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ArenaMono;
            var arenaMonoInit = Object.Instantiate(arenaMonoPrefab);
            arenaMonoInit.transform.position = _arenaPosition;
            
            var arenaData = new ArenaData {
                ArenaMono = arenaMonoInit,
            };
            
            _dataWorld.CreateOneData(arenaData);            
        }
        
        private void StartArenaBattle()
        {
            /*
            CutsceneArenaAction.CreateBattleData?.Invoke();
            CutsceneArenaAction.CreateUnitInArena?.Invoke();
            EnableVisualArena();
            
            _dataWorld.InitModule<ArenaModule>(true);
            */
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
            
            ControlViewGameUI(true);
            
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            PopupDistrictInfoAction.ClosePopup?.Invoke();
        }
        
        private void ControlViewGameUI(bool isOpenArena)
        {
            var uiCoreMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            
            if (isOpenArena)
            {
                uiCoreMono.TraderowMono.DisableTradeRow();
                uiCoreMono.CoreHudUIMono.HideEnemyPassport();
            }
            else
            {
                uiCoreMono.TraderowMono.EnableTradeRow();
                uiCoreMono.CoreHudUIMono.ShowEnemyPassport();
            }
        }
        
        private void EndBattleArena()
        {
            ClearArenaEndBattle();
            
            CityAction.UpdatePresencePlayerInCity?.Invoke();   
            _dataWorld.DestroyModule<ArenaModule>();
        }

        private void ClearArenaEndBattle()
        {
            //TODO Поправить, Арена
            /*
            var arenaData = _dataWorld.OneData<ArenaData>();
            var cameraData = _dataWorld.OneData<GameCameraData>();
            var cityData = _dataWorld.OneData<CityData>();

            ControlViewGameUI(false);
            AnimationsStartArenaCameraAction.ReturnCamera?.Invoke();
            arenaData.ArenaMono.DisableArena();
            arenaData.ArenaMono.DisableCamera();
            cameraData.CameraGO.SetActive(true);
            cityData.CityMono.OnCityLight();

            var playerInArenaEntities = _dataWorld.Select<PlayerArenaInBattleComponent>().GetEntities();
            
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
                unitEntity.RemoveComponent<ArenaUnitCurrentComponent>();
                unitEntity.RemoveComponent<UnitInBattleArenaComponent>();
            }
            
            var unitSelectForAttack = _dataWorld.Select<ArenaSelectUnitForAttackComponent>()
                .GetEntities();
            foreach (var unitEntity in unitSelectForAttack)
            {
                unitEntity.RemoveComponent<ArenaSelectUnitForAttackComponent>();
            }

            ref var roundData = ref _dataWorld.OneData<RoundData>();
            roundData.PauseInteractive = false;
            roundData.CurrentGameStateMapVSArena = GameStateMapVSArena.Map;
            VFXCardInteractiveAction.UpdateVFXCard?.Invoke();
            
            if (roundData.playerOrAI != PlayerOrAI.Player)
                BotAIAction.ContinuePlayingCards?.Invoke();
            else
            {
                ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
            }
            */
        }
        
        public void Destroy()
        {
            if (_dataWorld.IsModuleActive<ArenaModule>())
                _dataWorld.DestroyModule<ArenaModule>();
            
            CutsceneArenaAction.StartCutscene -= StartArenaBattle;

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