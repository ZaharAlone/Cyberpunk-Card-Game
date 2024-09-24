using System.Threading.Tasks;
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
using DG.Tweening;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityNoiseCardSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const string key_noise_card = "neutral_noise";
        
        public void PreInit()
        {
            AbilityCardAction.AddNoiseCard += AddNoiseCard;
            AbilityCardAction.CancelAddNoiseCard += CancelAddNoiseCard;
        }
        
        private void AddNoiseCard(string guidCard)
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.playerOrAI != PlayerOrAI.Player)
            {
                AbilityAIAction.AddNoiseSelectPlayer?.Invoke();
                return;
            }

            roundData.PauseInteractive = true;
            AbilityPopupUISystemAction.OpenPopupAbilityTargetInfo?.Invoke(AbilityType.AddNoiseCard, 0, true);
            BezierCurveNavigationAction.StartBezierCurveCard?.Invoke(guidCard, BezierTargetEnum.Player);
            //TODO Добавить выделение всех игроков, чтобы было понятно что мы выбираем из них
            EnemyPassportAction.OnClickPlayerPassport += SelectPlayerAddNoise;
        }
        
        private void SelectPlayerAddNoise(int targetPlayerID)
        {
            EnemyPassportAction.OnClickPlayerPassport -= SelectPlayerAddNoise;
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
            
            var cardsParent = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            var countCardPlayer = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == targetPlayerID)
                .Count();

            var cardData = new CardData();
            cardData.CardName = key_noise_card;
            cardData.IDPositions = countCardPlayer + 1;
            
            var noiseCardEntity = SetupCardAction.CreateCard.Invoke(cardData, cardsParent, true);
            
            ref var noiseCardComponent = ref noiseCardEntity.GetComponent<CardComponent>();
            noiseCardComponent.PlayerID = targetPlayerID;
            
            noiseCardEntity.AddComponent(new CardDiscardComponent());
            noiseCardEntity.AddComponent(new CardPlayerComponent());
            _dataWorld.OneData<RoundData>().PauseInteractive = false;

            AnimationsCard(noiseCardEntity, targetPlayerID);
            BoardGameUIAction.UpdateStatsAllPlayersPassportUI?.Invoke();

            EndPlayingAbility();
        }

        private async void AnimationsCard(Entity entityCard, int targetPlayerID)
        {
            var enemyPassport = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.EnemyPassports;
            var cardComponent = entityCard.GetComponent<CardComponent>();
            cardComponent.CardMono.CardOnFace();

            var targetPosition = Vector3.zero;
            for (int i = 0; i < enemyPassport.Count; i++)
            {
                if (enemyPassport[i].GetPlayerID() == targetPlayerID)
                    targetPosition = enemyPassport[i].transform.position;
            }

            await Task.Delay(200);
            var sequence = DOTween.Sequence();
            sequence.Append(cardComponent.CardMono.RectTransform.DOLocalRotate(new Vector3(0, 90, 0), 0.2f));
            await Task.Delay(200);
            
            cardComponent.CardMono.CardOnBack();
            sequence.Append(cardComponent.CardMono.RectTransform.DOLocalRotate(new Vector3(0, 180, 0), 0.2f));
            await Task.Delay(250);
                    
            sequence.Append(cardComponent.CardMono.transform.DOMove(targetPosition, 0.7f))
                .Join(cardComponent.CardMono.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.7f));

            await Task.Delay(250);
            sequence.Join(cardComponent.CardMono.BackCardImage.DOColor(new Color32(255, 255, 255, 0), 0.3f));
            await Task.Delay(400);
            sequence.Kill();
        }

        private void EndPlayingAbility()
        {
            //TODO не уверен что верно, но попробуем
            var cardComponent = _dataWorld.Select<CardComponent>()
                .With<AbilitySelectElementComponent>()
                .SelectFirstEntity()
                .GetComponent<CardComponent>();
            
            AbilityCardAction.CompletePlayingAbilityCard?.Invoke(cardComponent.GUID);   
        }

        private void CancelAddNoiseCard()
        {
            EnemyPassportAction.OnClickPlayerPassport -= SelectPlayerAddNoise;
            BezierCurveNavigationAction.OffBezierCurve?.Invoke();
        }

        public void Destroy()
        {
            AbilityCardAction.AddNoiseCard -= AddNoiseCard;
            AbilityCardAction.CancelAddNoiseCard -= CancelAddNoiseCard;
        }
    }
}