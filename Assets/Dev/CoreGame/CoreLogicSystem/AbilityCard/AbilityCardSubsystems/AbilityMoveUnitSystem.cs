using CyberNet.Core.AbilityCard.UI;
using CyberNet.Core.AI;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.City;
using CyberNet.Core.InteractiveCard;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using CyberNet.Global.Cursor;
using CyberNet.Global.GameCamera;
using CyberNet.Global.Sound;
using EcsCore;
using Input;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityMoveUnitSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            AbilityCardAction.MoveUnit += MoveUnit;
            AbilityCardAction.CancelMoveUnit += CancelMoveUnit;
        }

        private void MoveUnit(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.MoveUnit?.Invoke(guidCard);
                return;
            }

            _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity()
                .AddComponent(new AbilityCardMoveUnitComponent());
            
            AbilitySelectElementUIAction.OpenSelectAbilityCard?.Invoke(AbilityType.UnitMove, 0, false);
            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Tower);
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

            var currentPlayerID = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity()
                .GetComponent<PlayerComponent>()
                .PlayerID;
            
            CityAction.ShowWherePlayerCanMoveFrom?.Invoke(canMoveUnitComponent.SelectTowerGUID);
            CityAction.ActivationsColliderUnitsInTower?.Invoke(canMoveUnitComponent.SelectTowerGUID, currentPlayerID);
            AbilitySelectElementUIAction.OpenSelectAbilityCard?.Invoke(AbilityType.UnitMove, 1, false);
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
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.DeselectUnitInMap);
            }
            else
            {
                unitEntity.AddComponent(new SelectUnitMapComponent());
                unitComponent.IconsUnitInMapMono.OnSelectUnitEffect();
                SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.SelectUnitInMap);
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
            if (_dataWorld.Select<AbilityCardMoveUnitSelectTowerComponent>().Count() == 0
                || _dataWorld.OneData<RoundData>().CurrentGameStateMapVSArena == GameStateMapVSArena.Arena)
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
                    if (towerMono.GUID == abilityCardMoveUnitComponent.SelectTowerGUID && towerMono.IsInteractiveTower)
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

        private void SelectTowerToMove(string towerGUID)
        {
            var entityMoveCard = _dataWorld.Select<AbilityCardMoveUnitComponent>().SelectFirstEntity();
            ref var abilityCardMoveUnitComponent = ref entityMoveCard.GetComponent<AbilityCardMoveUnitComponent>();
            
            if (towerGUID != abilityCardMoveUnitComponent.SelectTowerGUID)
                return;
            
            CityAction.SelectUnit -= ClickOnUnit;
            CityAction.SelectTower -= SelectTowerToMove;
            ConfimMove();
        }

        private void ConfimMove()
        {
            MapMoveUnitsAction.StartMoveUnits?.Invoke();
            CityAction.DeactivationsColliderAllUnits?.Invoke();
            EndPlayingCard();
        }

        private void EndPlayingCard()
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .With<AbilityCardMoveUnitComponent>()
                .SelectFirstEntity();

            entityCard.RemoveComponent<AbilityCardMoveUnitComponent>();
            entityCard.RemoveComponent<SelectTargetCardAbilityComponent>();
            entityCard.RemoveComponent<CardHandComponent>();
            entityCard.RemoveComponent<InteractiveSelectCardComponent>();
            entityCard.RemoveComponent<CardComponentAnimations>();
            entityCard.RemoveComponent<AbilitySelectElementComponent>();
            entityCard.RemoveComponent<AbilityCardMoveUnitSelectTowerComponent>();
            
            entityCard.AddComponent(new CardStartMoveToTableComponent());

            CardAnimationsHandAction.AnimationsFanCardInHand?.Invoke();
            AnimationsMoveBoardCardAction.AnimationsMoveBoardCard?.Invoke();   
            
            AbilitySelectElementUIAction.ClosePopup?.Invoke();
            AbilityInputButtonUIAction.HideInputUIButton?.Invoke();
            CityAction.UpdateCanInteractiveMap?.Invoke();
            CityAction.UpdatePresencePlayerInCity?.Invoke();
            ActionPlayerButtonEvent.UpdateActionButton?.Invoke();
        }
        
        private void CancelMoveUnit(string guidCard)
        {
            var entityCard = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.GUID == guidCard)
                .SelectFirstEntity();

            //Убираем выделение с юнитов если какие-то юниты уже выделены
            var selectUnitsMapEntities = _dataWorld.Select<SelectUnitMapComponent>().GetEntities();

            foreach (var unitEntity in selectUnitsMapEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                unitEntity.RemoveComponent<SelectUnitMapComponent>();
                unitComponent.IconsUnitInMapMono.OffSelectUnitEffect();   
            }
            
            entityCard.RemoveComponent<AbilityCardMoveUnitComponent>();
            
            if (entityCard.HasComponent<SelectTargetCardAbilityComponent>())
                entityCard.RemoveComponent<SelectTargetCardAbilityComponent>();
            
            if (entityCard.HasComponent<AbilityCardMoveUnitSelectTowerComponent>())
                entityCard.RemoveComponent<AbilityCardMoveUnitSelectTowerComponent>();
            
            CityAction.DeactivateAllTower?.Invoke();
            CityAction.SelectTower -= SelectTower;
            CityAction.SelectUnit -= ClickOnUnit;
            CityAction.SelectTower -= SelectTowerToMove;
            CustomCursorAction.OnBaseCursor?.Invoke();
        }

        public void Destroy()
        {
            AbilityCardAction.MoveUnit -= MoveUnit;
            AbilityCardAction.CancelMoveUnit -= CancelMoveUnit;
            CityAction.SelectTower -= SelectTower;
        }
    }
}