using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.AI;
using CyberNet.Core.Arena;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.Cursor;
using CyberNet.Global.GameCamera;
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
        
        private void MoveUnit()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                AbilityAIAction.MoveUnit?.Invoke();
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

            var countUnitEntityIsDefensePosition = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDTower == selectTowerForAttackGuid).Count();

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
                
                //Animations move Unit to target zone
                //Start Attack
                EndPlayingCard();
                ArenaAction.StartArenaBattle?.Invoke();
            }
            else
            {
                //Animations move Unit to target zone
                //Switch new position
            }
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
            AbilityCancelButtonUIAction.HideCancelButton?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
            CityAction.UpdatePresencePlayerInCity?.Invoke();
        }

        private void AnimationMoveToTarget()
        {
            
        }

        private void FinishAnimation()
        {
            
        }
    }
}