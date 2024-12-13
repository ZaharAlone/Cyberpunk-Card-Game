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

namespace CyberNet.Core.AbilityCard.DiscardCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityDiscardCardSelectPlayer : IPreInitSystem, IDestroySystem
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

            if (playerEntity.HasComponent<PlayerEffectDiscardCardComponent>())
            {
                ref var playerDiscardCardComponent = ref playerEntity.GetComponent<PlayerEffectDiscardCardComponent>();
                playerDiscardCardComponent.Count++;
            }
            else
            {
                playerEntity.AddComponent(new PlayerEffectDiscardCardComponent {Count = 1});
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

        public void Destroy()
        {
            AbilityCardAction.DiscardCard -= StartAbilityDiscardCard;
            EnemyPassportAction.OnClickPlayerPassport -= OnClickPlayerPassport;
            AbilityCardAction.CancelDiscardCard -= CancelDiscardCard;
        }
    }
}