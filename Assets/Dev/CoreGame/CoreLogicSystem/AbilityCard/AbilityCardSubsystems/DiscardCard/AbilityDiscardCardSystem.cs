using CyberNet.Core.AI.Ability;
using CyberNet.Core.BezierCurveNavigation;
using CyberNet.Core.EnemyPassport;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDiscardCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.DiscardCard += StartAbilityDiscardCard;
            AbilityCardAction.CancelDiscardCard += CancelDiscardCard;
        }

        private void StartAbilityDiscardCard(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.DiscardCardSelectPlayer?.Invoke();
                return;
            }

            //Показываем попап и включаем vfx выделения игроков
            //TODO Сделать корректный попап
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(AbilityType.EnemyDiscardCard, 0, true);
            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Player);
            //TODO Добавить выделение всех игроков, чтобы было понятно что мы выбираем из них
            EnemyPassportAction.OnClickPlayerPassport += OnClickPlayerPassport;
        }

        private void OnClickPlayerPassport(int targetPlayerID)
        {
            EnemyPassportAction.OnClickPlayerPassport -= OnClickPlayerPassport;
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
            
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == targetPlayerID)
                .SelectFirstEntity();

            if (playerEntity.HasComponent<PlayerDiscardCardComponent>())
            {
                ref var playerDiscardCardComponent = ref playerEntity.GetComponent<PlayerDiscardCardComponent>();
                playerDiscardCardComponent.Count++;
            }
            else
            {
                playerEntity.AddComponent(new PlayerDiscardCardComponent {Count = 1});
            }
            
            _dataWorld.OneData<RoundData>().PauseInteractive = false;
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();

            EndPlayingAbility();
        }

        private void EndPlayingAbility()
        {
            var cardComponent = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity()
                .GetComponent<CardComponent>();
            
            AbilityCardAction.CompletePlayingAbilityCard?.Invoke(cardComponent.GUID);
        }
        
        private void CancelDiscardCard()
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = false;
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
            EnemyPassportAction.OnClickPlayerPassport -= OnClickPlayerPassport;
        }

        /*
        private void DiscardCard(Entity entity)
        {
            ref var playerTargetDiscard = ref entity.GetComponent<PlayerDiscardCardComponent>().TargetDiscardCardID;
            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            ref var currentPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;
            
            Object.Destroy(gameUI.CoreHudUIMono.PlayerDownView.FrameEffectCard.GetChild(0).gameObject);
        }*/
/*

        //NOT WORK RUN
        public void Run()
        {
            var entities = _dataWorld.Select<ActionCardDiscardCardVisualEffect>()
                .Without<CardComponentAnimations>()
                .GetEntities();

            foreach (var entity in entities)
            {
                DiscardCardEffect(entity);
            }
        }

        private void DiscardCardEffect(Entity entity)
        {
            ref var playerTargetDiscard = ref entity.GetComponent<ActionCardDiscardCardVisualEffect>().TargetDiscardCardPlayerID;
            var abilityVFX = _dataWorld.OneData<ActionCardConfigData>().ActionCardConfig;
            ref var cardComponent = ref entity.GetComponent<CardComponent>();
            ref var cardsContainer = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            
            ActionCardVisualEffect.CreateEffect(abilityVFX.discardActionCardVFX, cardComponent.RectTransform.position, cardsContainer);

            ref var gameUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;
            ref var currentPlayerID = ref _dataWorld.OneData<RoundData>().CurrentPlayerID;

            //TODO: старый код
            if (currentPlayerID == playerTargetDiscard)
            {
                Object.Instantiate(abilityVFX.DiscardCardUIEffect, gameUI.CoreHudUIMono.PlayerDownView.FrameEffectCard);
            }
            else
            {
                Object.Instantiate(abilityVFX.DiscardCardUIEffect, gameUI.CoreHudUIMono.PlayerUpView.FrameEffectCard);
            }
            entity.RemoveComponent<ActionCardDiscardCardVisualEffect>();

            var newEntity = _dataWorld.NewEntity();
            newEntity.AddComponent(new PlayerDiscardCardComponent {
                TargetDiscardCardID = playerTargetDiscard
            });
        }
        */

        public void Destroy()
        {
            AbilityCardAction.DiscardCard -= StartAbilityDiscardCard;
            EnemyPassportAction.OnClickPlayerPassport -= OnClickPlayerPassport;
            AbilityCardAction.CancelDiscardCard -= CancelDiscardCard;
        }
    }
}