using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.City;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global.Cursor;
using CyberNet.Global.GameCamera;
using Input;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaPlayerControlSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;

        /* Старт хода
         * 3) Даем управление игроку, игрок может выбрать как цель юнита противника и атаковать его
         * 4) ... разыгрыш карт пока убираем, но нужно дописать логику которая убирает выделение с карт,
         *        которые нельзя играть на арене
         * 5) Игрок может выбрать как цель противника, и закончить ход
         * 6) Игрок может спасовать - пока не делаем
         *
         * 7) Описываем поведение нейтрального юнита
         * 8) Создаем цикл с перестрелкой и захватом территории
         */
        private bool _isAim;
        
        public void PreInit()
        {
            ArenaUIAction.ClickAttack += ClickAttack;
        }

        public void Run()
        {
            PlayerSelectEnemy();
        }

        private void PlayerSelectEnemy()
        {
            //Держим в уме отбирать у игрока управление если не его ход
            
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

        private void ClickInUnit(UnitArenaMono unitMono)
        {
            var colorsConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ColorsGameConfigSO;
            
            //Проверяем, есть ли уже выбранные юниты
            var isSelectUnitForAttack = _dataWorld.Select<ArenaSelectUnitForAttackComponent>()
                .TrySelectFirstEntity(out var entitySelectUnit);

            if (isSelectUnitForAttack)
            {
                entitySelectUnit.RemoveComponent<ArenaSelectUnitForAttackComponent>();
                var selectUnitComponent = entitySelectUnit.GetComponent<ArenaUnitComponent>();
                
                selectUnitComponent.UnitArenaMono.UnitPointVFXMono.SetColor(colorsConfig.BaseColor);
                selectUnitComponent.UnitArenaMono.UnitPointVFXMono.DisableEffect();
            }

            var unitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.GUID == unitMono.GUID)
                .SelectFirstEntity();
            unitEntity.AddComponent(new ArenaSelectUnitForAttackComponent());

            unitMono.UnitPointVFXMono.SetColor(colorsConfig.SelectWrongTargetRedColor);
            unitMono.UnitPointVFXMono.EnableEffect();
        }
        
        private void ClickAttack()
        {
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
            
            var isEnemyAttack = _dataWorld.Select<ArenaSelectUnitForAttackComponent>()
                .TrySelectFirstEntity(out var unitAttackEntity);

            if (!isEnemyAttack)
            {
                //Show Warning frame
                Debug.LogError("Not select unit for attack");
            }
            else
            {
                //Reaction Stage

                var unitComponent = unitAttackEntity.GetComponent<ArenaUnitComponent>();
                if (unitComponent.PlayerControlEnum == PlayerControlEnum.Neutral)
                {
                    EndReactionStage();
                    ArenaUIAction.HideHUDButton?.Invoke();
                }
                else
                {
                    //Reaction stage other player
                }
                
            }
        }
        private void EndReactionStage()
        {
            ArenaAction.ArenaUnitStartAttack?.Invoke();
        }

        private void ClearStage()
        {
            
        }
    }
}