using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;

namespace BoardGame.Core.UI
{
    [EcsSystem(typeof(BoardGameModule))]
    public class BoardInteractiveSystem : IRunSystem, IPostRunSystem
    {
        private DataWorld _dataWorld;

        public void Run()
        {
            var entities = _dataWorld.Select<CardComponent>().With<InteractiveMoveComponent>().GetEntities();

            foreach (var entity in entities)
            {
                ref var componentMove = ref entity.GetComponent<InteractiveMoveComponent>();
                ref var componentCard = ref entity.GetComponent<CardComponent>();

                var distance = componentCard.Transform.position.y - componentMove.StartCardPosition.y;
                var ui = _dataWorld.OneData<BoardGameUIComponent>();

                if (distance > 150)
                    ui.UIMono.InteractiveZoneImage.color = new Color(255, 255, 255, 255);
                else
                    ui.UIMono.InteractiveZoneImage.color = new Color(255, 255, 255, 0);
            }
        }

        public void PostRun()
        {
            var updateUI = _dataWorld.Select<EventUpdateBoardCardUI>().Count();

            if (updateUI > 0)
                UpdateUI();
        }

        private void UpdateUI()
        {
            var countCard = _dataWorld.Select<CardPlayerComponent>().With<CardComponent>().With<CardInDeckComponent>().Count();
            var entities = _dataWorld.Select<CardComponent>().With<CardPlayerComponent>().With<CardInDeckComponent>().GetEntities();
            var config = _dataWorld.OneData<BoardGameData>().BoardGameConfig;

            Debug.Log(countCard);
            var width = (204 + 30) * (countCard - 1);
            var start_point = width / -2;
            Debug.Log(start_point);

            foreach (var entity in entities)
            {
                ref var card = ref entity.GetComponent<CardComponent>();
                card.Transform.rotation = Quaternion.identity;
                card.Transform.position = config.PlayerCardPositionInPlay;
                var pos = card.Transform.position;
                pos.x = start_point;
                card.Transform.position = pos;

                start_point += (204 + 30);
            }
        }
    }
}