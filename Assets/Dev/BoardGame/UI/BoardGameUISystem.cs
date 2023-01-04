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
    public class BoardGameUISystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            var gameUI = _dataWorld.GetOneData<BoardGameUIComponent>().GetData();
            var camera = _dataWorld.GetOneData<BoardGameCameraComponent>().GetData();

            var canvas = gameUI.UIGO.GetComponent<Canvas>();
            canvas.worldCamera = camera.MainCamera;
        }
    }
}