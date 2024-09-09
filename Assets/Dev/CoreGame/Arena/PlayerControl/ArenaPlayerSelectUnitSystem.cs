using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Arena;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Global.Cursor;
using CyberNet.Global.Sound;
using Input;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaPlayerSelectUnitSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private bool _isAim;

        public void PreInit()
        {
            ArenaAction.SelectUnitEnemyTargetingPlayer += SelectUnitEnemyTargetingPlayer;
        }

        //Находим первого противника и выбираем его как цель в начале раунда стрельбы игрока
        private void SelectUnitEnemyTargetingPlayer()
        {
            //Находим первого попавшегося противника на арене
            var selectEnemyPlayerEntity = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Without<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var selectEnemyPlayerComponent = selectEnemyPlayerEntity.GetComponent<PlayerArenaInBattleComponent>();

            var selectEnemyUnitComponent = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == selectEnemyPlayerComponent.PlayerID)
                .SelectFirst<ArenaUnitComponent>();
            
            AimToUnit(selectEnemyUnitComponent.UnitArenaMono);
        }
        
        public void Run()
        {
            if (_dataWorld.Select<PlayerStageChoosesAnOpponentComponent>().Count() == 0
                || _dataWorld.Select<SelectTargetCardAbilityComponent>().Count() != 0)
                return;
            
            PlayerSelectEnemy();
        }

        private void PlayerSelectEnemy()
        {
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<ArenaData>().ArenaMono.ArenaCameraMono.ArenaCamera;
            var ray = camera.ScreenPointToRay(inputData.MousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var unitArenaMono = hit.collider.gameObject.GetComponent<UnitArenaMono>();
                if (unitArenaMono)
                {
                    if (!_isAim)
                    {
                        CustomCursorAction.OnAimCursor?.Invoke();
                        _isAim = true;
                    }

                    if (inputData.Click)
                    {
                        AimToUnit(unitArenaMono);
                    }
                }
                else
                {
                    if (_isAim)
                    {
                        CustomCursorAction.OnBaseCursor?.Invoke();
                        _isAim = false;
                    }
                }
            }
        }

        private void AimToUnit(UnitArenaMono unitMonoTarget)
        {
            var colorsConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ColorsGameConfigSO;
            
            //Проверяем, есть ли уже выбранные юниты
            var isSelectUnitForAttack = _dataWorld.Select<ArenaSelectUnitForAttackComponent>()
                .TrySelectFirstEntity(out var entitySelectUnit);

            if (isSelectUnitForAttack)
            {
                entitySelectUnit.RemoveComponent<ArenaSelectUnitForAttackComponent>();
                var selectUnitComponent = entitySelectUnit.GetComponent<ArenaUnitComponent>();
                
                selectUnitComponent.UnitArenaMono.UnitPointVFXMono.SetColor(colorsConfig.BaseColor, false);
                selectUnitComponent.UnitArenaMono.UnitPointVFXMono.DisableEffect();
            }

            var unitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.GUID == unitMonoTarget.GUID)
                .SelectFirstEntity();
            unitEntity.AddComponent(new ArenaSelectUnitForAttackComponent());

            unitMonoTarget.UnitPointVFXMono.SetColor(colorsConfig.SelectWrongTargetRedColor, true);
            unitMonoTarget.UnitPointVFXMono.EnableEffect();

            var currentUnitComponent = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity().GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.ViewToTargetUnit(unitMonoTarget.transform);
            currentUnitComponent.UnitArenaMono.OnAimAnimations();

            var soundAim = _dataWorld.OneData<SoundData>().Sound.AimGun;
            SoundAction.PlaySound?.Invoke(soundAim);
        }

        public void Destroy()
        {
            ArenaAction.SelectUnitEnemyTargetingPlayer -= SelectUnitEnemyTargetingPlayer;
        }
    }
}