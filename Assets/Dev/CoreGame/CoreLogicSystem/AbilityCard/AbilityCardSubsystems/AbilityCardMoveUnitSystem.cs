using System.Collections.Generic;
using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.AI;
using CyberNet.Core.Arena;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.Cursor;
using CyberNet.Global.GameCamera;
using DG.Tweening;
using EcsCore;
using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityCardMoveUnitSystem : IPreInitSystem, IRunSystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            AbilityCardAction.MoveUnit += MoveUnit;
        }
        
        private void MoveUnit(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                Debug.LogError("move unit ai");
                AbilityAIAction.MoveUnit?.Invoke(guidCard);
                return;
            }

            _dataWorld.NewEntity().AddComponent(new AbilityCardMoveUnitComponent());

            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity();
            var cardComponent = entityCard.GetComponent<CardComponent>();
            var cardPosition = cardComponent.RectTransform.position;
            cardPosition.y += cardComponent.RectTransform.sizeDelta.y / 2;
            
            roundData.PauseInteractive = true;
            AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.SquadMove, 0, false);
            BezierCurveNavigationAction.StartBezierCurve?.Invoke(cardPosition, BezierTargetEnum.Tower);
            CityAction.ShowWherePlayerCanMove?.Invoke();
            CityAction.SelectTower += SelectTower;
        }
        
        private void SelectTower(string towerGUID)
        {
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
            CityAction.SelectTower -= SelectTower;
            
            var entityMoveCard = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity();
            ref var moveCardComponent = ref entityMoveCard.GetComponent<AbilityCardMoveUnitComponent>();
            moveCardComponent.SelectTowerGUID = towerGUID;

            entityMoveCard.AddComponent(new AbilityCardMoveUnitSelectTowerComponent());
            CityAction.UpdateCanInteractiveMap?.Invoke();

            FollowSelectUnitToMove();
        }

        private void FollowSelectUnitToMove()
        {
            var canMoveUnitComponent = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity()
                .GetComponent<AbilityCardMoveUnitComponent>();
            
            CityAction.ShowWherePlayerCanMoveFrom?.Invoke(canMoveUnitComponent.SelectTowerGUID);
            AbilitySelectElementAction.OpenSelectAbilityCard?.Invoke(AbilityType.SquadMove, 1, false);
            CityAction.SelectUnit += ClickOnUnit;
        }

        private void ClickOnUnit(string unitGUID)
        {
            var unitEntity = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDUnit == unitGUID)
                .SelectFirstEntity();

            var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
            
            if (unitEntity.HasComponent<SelectUnitMapComponent>())
            {
                unitEntity.RemoveComponent<SelectUnitMapComponent>();
                unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();
            }
            else
            {
                unitEntity.AddComponent(new SelectUnitMapComponent());
                unitComponent.IconsUnitInMapMono.OnSelectUnitEffect();
            }

            CheckUpdateReadinessUnitsForShipment();
        }

        //Проверяем могут ли быть отправлены отряды сейчас
        private void CheckUpdateReadinessUnitsForShipment()
        {
            var countSelectUnit = _dataWorld.Select<SelectUnitMapComponent>()
                .Count();

            var entityMoveCard = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity();
            var targetTowerGUID = entityMoveCard.GetComponent<AbilityCardMoveUnitComponent>().SelectTowerGUID;
            
            if (countSelectUnit > 0)
            {
                CityAction.EnableInteractiveTower?.Invoke(targetTowerGUID);
            }
            else
            {
                CityAction.DisableInteractiveTower?.Invoke(targetTowerGUID);
            }
        }
        
        public void Run()
        {
            if (_dataWorld.Select<AbilityCardMoveUnitSelectTowerComponent>().Count() == 0)
                return;

            DrawTargetStartAttack();
        }
        
        private void DrawTargetStartAttack()
        {
            var countSelectUnit = _dataWorld.Select<SelectUnitMapComponent>()
                .Count();
            
            if (countSelectUnit == 0)
                return;
            
            var entityMoveCard = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity();
            ref var abilityCardMoveUnitComponent = ref entityMoveCard.GetComponent<AbilityCardMoveUnitComponent>();
                
            var inputData = _dataWorld.OneData<InputData>();
            var camera = _dataWorld.OneData<GameCameraData>();
            var ray = camera.MainCamera.ScreenPointToRay(inputData.MousePosition);
            var isCurrentTowerSelect = false;
            
            if (Physics.Raycast(ray, out RaycastHit hit, 1500f))
            {
                var towerMono = hit.collider.gameObject.GetComponent<TowerMono>();
                if (towerMono)
                {
                    if (towerMono.GUID == abilityCardMoveUnitComponent.SelectTowerGUID)
                    {
                        isCurrentTowerSelect = true;
                        if (!abilityCardMoveUnitComponent.IsAimOn)
                        {
                            abilityCardMoveUnitComponent.IsAimOn = true;
                            CustomCursorAction.OnAimCursor?.Invoke();
                            CityAction.SelectTower += SelectTowerToMove;
                        }
                    }
                }
            }
            
            if (!isCurrentTowerSelect && abilityCardMoveUnitComponent.IsAimOn)
            {
                CustomCursorAction.OnBaseCursor?.Invoke();
                abilityCardMoveUnitComponent.IsAimOn = false;
                CityAction.SelectTower -= SelectTowerToMove;
            }
        }
        
        private void SelectTowerToMove(string guid)
        {
            var entityMoveCard = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity();
            ref var abilityCardMoveUnitComponent = ref entityMoveCard.GetComponent<AbilityCardMoveUnitComponent>();
            
            if (guid != abilityCardMoveUnitComponent.SelectTowerGUID)
                return;
            
            CityAction.SelectUnit -= ClickOnUnit;
            CityAction.SelectTower -= SelectTowerToMove;
            ConfimMove();
        }

        private void ConfimMove()
        {
            var entityMoveCard = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity();
            var selectUnitEntities = _dataWorld.Select<SelectUnitMapComponent>().GetEntities();
            var selectTowerForAttackGuid = entityMoveCard.GetComponent<AbilityCardMoveUnitComponent>().SelectTowerGUID;
            
            var targetTowerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == selectTowerForAttackGuid)
                .SelectFirstEntity();
            var targetTowerComponent = targetTowerEntity.GetComponent<TowerComponent>();
            
            var countUnitEntityIsDefensePosition = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDTower == selectTowerForAttackGuid).Count();

            var targetSlotZone = 0;
            if (countUnitEntityIsDefensePosition > 0)
            {
                var entitiesUnitEntityIsDefensePosition = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == selectTowerForAttackGuid)
                    .GetEntities();
                
                foreach (var entity in entitiesUnitEntityIsDefensePosition)
                {
                    entity.AddComponent(new UnitInBattleArenaComponent {
                        Forwards = false
                    });
                }
                
                foreach (var entity in selectUnitEntities)
                {
                    entity.AddComponent(new UnitInBattleArenaComponent {
                        Forwards = true
                    });
                }

                targetSlotZone = GetEnemySlotInTargetZone(selectTowerForAttackGuid);
            }
            else
            {
                targetSlotZone = GetFriendlySlotInTargetZone(selectTowerForAttackGuid);
            }
            
            var positionAllPoint = new List<Vector3>();
            foreach (var unitEntity in selectUnitEntities)
            {
                unitEntity.RemoveComponent<SelectUnitMapComponent>();
                ref var unitComponent = ref unitEntity.GetComponent<UnitMapComponent>();
                
                var targetPosition = CitySupportStatic.SelectPosition
                (
                    targetTowerComponent.SquadZonesMono[targetSlotZone].Collider,
                    targetTowerComponent.SquadZonesMono[targetSlotZone].transform.position,
                    positionAllPoint
                );
                positionAllPoint.Add(targetPosition);
                
                unitEntity.AddComponent(new MoveUnitToTargetComponent {
                    TargetPosition = targetPosition,
                    TargetTowerGUID = selectTowerForAttackGuid,
                    TargetSlotID = targetSlotZone
                });

                unitComponent.GUIDTower = selectTowerForAttackGuid;
                unitComponent.IndexPoint = targetSlotZone;
                
                unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();
                unitComponent.UnitIconsGO.transform.SetParent(targetTowerComponent.SquadZonesMono[targetSlotZone].transform);
            }
            
            EndPlayingCard();
            AnimationMoveToTarget();
        }

        private void EndPlayingCard()
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .Where<AbilitySelectElementComponent>(selectCard => selectCard.AbilityCard.AbilityType == AbilityType.SquadMove)
                .SelectFirstEntity();
            
            entityCard.RemoveComponent<AbilitySelectElementComponent>();
            entityCard.RemoveComponent<AbilitySelectElementComponent>();
            entityCard.RemoveComponent<AbilityCardAddUnitComponent>();
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            
            entityCard.AddComponent(new CardMoveToTableComponent());
            
            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
            AbilitySelectElementAction.ClosePopup?.Invoke();
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
            CityAction.UpdatePresencePlayerInCity?.Invoke();
        }

        private int GetFriendlySlotInTargetZone(string towerGUID)
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
            
            var targetSquadZone = 0;
            foreach (var squadZone in towerComponent.SquadZonesMono)
            {
                var isFriendlySlot = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID == currentPlayerComponent.PlayerID)
                    .Count() > 0;

                if (isFriendlySlot)
                    break;
                else
                {
                    targetSquadZone = squadZone.Index + 1;
                }
            }
            
            return targetSquadZone;
        }
        
        private int GetEnemySlotInTargetZone(string towerGUID)
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerComponent>();
            
            var towerEntity = _dataWorld.Select<TowerComponent>()
                .Where<TowerComponent>(tower => tower.GUID == towerGUID)
                .SelectFirstEntity();
            ref var towerComponent = ref towerEntity.GetComponent<TowerComponent>();
            
            var targetSquadZone = 0;
            foreach (var squadZone in towerComponent.SquadZonesMono)
            {
                var isCloseSlot = _dataWorld.Select<UnitMapComponent>()
                    .Where<UnitMapComponent>(unit => unit.GUIDTower == towerGUID
                        && unit.IndexPoint == squadZone.Index
                        && unit.PowerSolidPlayerID != currentPlayerComponent.PlayerID)
                    .Count() > 0;

                if (isCloseSlot)
                    targetSquadZone = squadZone.Index + 1;
                else
                {
                    break;
                }
            }
            
            return targetSquadZone;
        }
        
        private void AnimationMoveToTarget()
        {
            var moveUnitToTargetEntities = _dataWorld.Select<MoveUnitToTargetComponent>().GetEntities();

            foreach (var unitEntity in moveUnitToTargetEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                var unitMoveComponent = unitEntity.GetComponent<MoveUnitToTargetComponent>();

                var timeMove = 1;
                var sequence = DOTween.Sequence();
                sequence.Append(unitComponent.UnitIconsGO.transform.DOMove(unitMoveComponent.TargetPosition, timeMove));

                unitEntity.AddComponent(new TimeComponent {
                    Time = timeMove,
                    Action = () => {
                        unitEntity.RemoveComponent<MoveUnitToTargetComponent>();
                        CheckFinishMoveUnit();
                    }
                });
            }
        }

        private void CheckFinishMoveUnit()
        {
            var isMoveUnit = _dataWorld.Select<MoveUnitToTargetComponent>().Count() > 0;

            if (!isMoveUnit)
            {
                ZoomCameraToBattle();
            }
        }

        private void ZoomCameraToBattle()
        {
            AnimationsStartArenaCameraAction.StartAnimations?.Invoke(1f);

            var entity = _dataWorld.NewEntity();
            entity.AddComponent(new TimeComponent {
                Time = 1f,
                Action = () => {
                    entity.Destroy();
                    FinishAnimation();
                }
            });
        }

        private void FinishAnimation()
        {
            ArenaAction.StartArenaBattle?.Invoke();
        }
    }
}