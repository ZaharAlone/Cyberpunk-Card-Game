using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFramework.Data.Enumerators;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class HandUISystem : IPostRunEventSystem<EventUpdateHandUI>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventUpdateHandUI _) => UpdateUI();

        private void UpdateUI()
        {
            var round = _dataWorld.OneData<RoundData>();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;
            var uiRect = _dataWorld.OneData<BoardGameUIComponent>().UIMono.UIRect;

            if (round.CurrentPlayer == PlayerEnum.Player)
            {
                var countCardInHand = _dataWorld.Select<CardPlayerComponent>().With<CardHandComponent>().Count();
                var entities = _dataWorld.Select<CardPlayerComponent>().With<CardHandComponent>().GetEntities();
                var position = new Vector2(0, -uiRect.rect.height / 2 + 142);
                UpdateView(entities, countCardInHand, position, config.StepPosXPlayer);
            }
            else
            {
                var countCardInHand = _dataWorld.Select<CardEnemyComponent>().With<CardHandComponent>().Count();
                var entities = _dataWorld.Select<CardEnemyComponent>().With<CardHandComponent>().GetEntities();
                var position = new Vector2(0, uiRect.rect.height / 2 - 142 * config.SizeCardEnemy.y);
                UpdateView(entities, countCardInHand, position, config.StepPosXEnemy);
            }
        }

        private void UpdateView(EntitiesEnumerable entities, int countCardInHand, Vector2 position, float stepX)
        {
            foreach (var entity in entities)
            {
                ref var card = ref entity.GetComponent<CardComponent>();
                card.Transform.rotation = Quaternion.identity;
                card.Transform.position = position;
            }

            var posX = (float)(countCardInHand - 1) / 2 * (-stepX);

            var index = 0;
            foreach (var entity in entities)
            {
                ref var cardComponent = ref entity.GetComponent<CardComponent>();

                cardComponent.Transform.position = new Vector3(cardComponent.Transform.position.x + posX,
                                                               cardComponent.Transform.position.y,
                                                               cardComponent.Transform.position.z);
                cardComponent.Canvas.sortingOrder = 2 + index;
                index++;

                posX += stepX;
            }
        }
    }
}