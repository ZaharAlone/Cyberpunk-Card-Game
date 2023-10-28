/*
using System.Threading.Tasks;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.AI;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;
using DG.Tweening;
using UnityEngine;

namespace CyberNet.Core.AbilityCard
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityNoiseCardSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityCardAction.AddNoiseCard += AddNoiseCard;
        }
        
        private void AddNoiseCard()
        {
            ref var roundData = ref _dataWorld.OneData<RoundData>();

            if (roundData.PlayerTypeEnum != PlayerTypeEnum.Player)
            {
                AbilityAIAction.AddNoiseSelectPlayer?.Invoke();
                return;
            }

            roundData.PauseInteractive = true;
            AbilitySelectElementAction.SelectEnemyPlayer?.Invoke(AbilityType.AddNoiseCard);
            AbilityCardAction.SelectPlayer += SelectPlayerAddNoise;
        }
        
        private void SelectPlayerAddNoise(int targetPlayerID)
        {
            AbilityCardAction.SelectPlayer -= SelectPlayerAddNoise;
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == targetPlayerID)
                .SelectFirstEntity();
            
            var cardsParent = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CardsContainer;
            var countCardPlayer = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == targetPlayerID)
                .Count();

            var cardData = new CardData();
            cardData.CardName = "neutral_noise";
            cardData.IDPositions = countCardPlayer + 1;
            var noiseCardEntity = SetupCardAction.InitCard.Invoke(cardData, cardsParent, false);
            noiseCardEntity.AddComponent(new CardDiscardComponent());
            _dataWorld.OneData<RoundData>().PauseInteractive = false;

            AnimationsCard(noiseCardEntity, targetPlayerID);
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
    }
}*/