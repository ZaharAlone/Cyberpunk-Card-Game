using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFramework.Systems.Events;
using ModulesFrameworkUnity;
using System.Collections.Generic;
using UnityEngine;
using ModulesFramework.Data.Enumerators;
using DG.Tweening;
using System.Threading.Tasks;

//TODO: старый код
/*
namespace CyberNet.Core.UI
{
    [EcsSystem(typeof(CoreModule))]
    public class BoardZoneMoveToPlayCardSystem : IRunSystem
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
                var ui = _dataWorld.OneData<CoreUIData>();

                if (distance > 150)
                    ui.BoardGameUIMono.CoreHudUIMono.InteractiveZoneImage.color = new Color(255, 255, 255, 255);
                else
                    ui.BoardGameUIMono.CoreHudUIMono.InteractiveZoneImage.color = new Color(255, 255, 255, 0);
            }
        }
    }
}*/