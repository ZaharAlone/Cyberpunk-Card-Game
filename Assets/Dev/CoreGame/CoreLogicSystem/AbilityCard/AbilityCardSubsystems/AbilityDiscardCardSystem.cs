using CyberNet.Core.AI;
using CyberNet.Core.AI.Ability;
using CyberNet.Core.BezierCurveNavigation;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDiscardCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.DiscardCard += DiscardCardAbility;
            AbilityCardAction.CancelDiscardCard += CancelDiscardCard;
        }

        private void DiscardCardAbility(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.DiscardCardSelectPlayer?.Invoke();
                return;
            }

            //Показываем попап и включаем vfx выделения игроков
            AbilitySelectElementUIAction.SelectEnemyPlayer?.Invoke(AbilityType.EnemyDiscardCard);
            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Tower);
            AbilityCardAction.SelectPlayer += SelectPlayerDiscardCard;
        }

        private void SelectPlayerDiscardCard(int targetPlayerID)
        {
            AbilityCardAction.SelectPlayer -= SelectPlayerDiscardCard;
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
        }
        
        private void CancelDiscardCard()
        {
            _dataWorld.OneData<RoundData>().PauseInteractive = false;
            AbilityCardAction.SelectPlayer -= SelectPlayerDiscardCard;
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
            AbilityCardAction.DiscardCard -= DiscardCardAbility;
            AbilityCardAction.CancelDiscardCard -= CancelDiscardCard;
        }
    }
}