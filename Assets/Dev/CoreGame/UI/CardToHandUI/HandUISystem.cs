using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFramework.Data.Enumerators;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class HandUISystem : IPostRunEventSystem<EventUpdateHandUI>
    {
        private DataWorld _dataWorld;

        public void PostRunEvent(EventUpdateHandUI _) => UpdateUI();

        private void UpdateUI()
        {
            var round = _dataWorld.OneData<RoundData>();

            if (round.CurrentPlayer == PlayerEnum.Player1)
            {
                var countCardInHand = _dataWorld.Select<CardPlayer1Component>().With<CardHandComponent>().Count();
                var entities = _dataWorld.Select<CardPlayer1Component>().With<CardHandComponent>().GetEntities();
                UpdateView(entities, countCardInHand, round.CurrentPlayer);
            }
            else
            {
                var countCardInHand = _dataWorld.Select<CardPlayer2Component>().With<CardHandComponent>().Count();
                var entities = _dataWorld.Select<CardPlayer2Component>().With<CardHandComponent>().GetEntities();
                UpdateView(entities, countCardInHand, round.CurrentPlayer);
            }
        }

        private void UpdateView(EntitiesEnumerable entities, int countCardInHand, PlayerEnum isPlayer)
        {
            var viewPlayer = _dataWorld.OneData<ViewPlayerData>();
            var uiRect = _dataWorld.OneData<UIData>().UIMono.UIRect;
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            var position = new Vector2();
            var stepX = 0f;

            if (viewPlayer.PlayerView == isPlayer)
            {
                position = new Vector2(0, -uiRect.rect.height / 2 + 142);
                stepX = config.StepPosXPlayerDown;
            }
            else
            {
                position = new Vector2(0, uiRect.rect.height / 2 - 142 * config.SizeCardPlayerUp.y);
                stepX = config.StepPosXPlayerUp;
            }

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