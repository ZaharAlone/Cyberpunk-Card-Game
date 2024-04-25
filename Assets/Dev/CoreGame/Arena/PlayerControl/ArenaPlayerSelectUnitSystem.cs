using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Arena;
using CyberNet.Global.Cursor;
using CyberNet.Global.Sound;
using Input;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaPlayerSelectUnitSystem : IRunSystem
    {
        private DataWorld _dataWorld;
        private bool _isAim;
        
        public void Run()
        {
            if (_dataWorld.Select<PlayerStageChoosesAnOpponentComponent>().Count() == 0)
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
                        ClickInUnit(unitArenaMono);
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

        private void ClickInUnit(UnitArenaMono unitMonoTarget)
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
    }
}