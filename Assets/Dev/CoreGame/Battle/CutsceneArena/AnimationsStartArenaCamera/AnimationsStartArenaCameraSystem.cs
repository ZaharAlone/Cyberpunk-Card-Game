using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Global.GameCamera;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class AnimationsStartArenaCameraSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private float _valueFoVCamera;
        
        /// <summary>
        /// Общая логика работы
        /// - Запоминаем текущие FoV, скейлим на определенное значение за N времени
        /// - Затем перестаем скейлить, переходим на арену.
        /// - После окончания арены возвращаемся в первоначальный скейл.
        /// </summary>
        
        public void PreInit()
        {
            AnimationsStartArenaCameraAction.StartAnimations += StartAnimations;
            AnimationsStartArenaCameraAction.ReturnCamera += ReturnCameraViewArena;
        }

        private void StartAnimations(float time)
        {
            var camera = _dataWorld.OneData<GameCameraData>();
            _valueFoVCamera = camera.CoreVirtualCamera.m_Lens.FieldOfView;
            
            var entity = _dataWorld.NewEntity();
            entity.AddComponent(new AnimationsStartArenaCameraComponent {
                Time = time
            });
        }

        private void ReturnCameraViewArena()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            camera.CoreVirtualCamera.m_Lens.FieldOfView = _valueFoVCamera;
        }
        
        public void Run()
        {
            if (_dataWorld.Select<AnimationsStartArenaCameraComponent>().Any())
                AnimationCamera();
        }
        
        private void AnimationCamera()
        {
            ref var animationsComponent = ref _dataWorld.Select<AnimationsStartArenaCameraComponent>()
                .SelectFirstEntity()
                .GetComponent<AnimationsStartArenaCameraComponent>();

            animationsComponent.Time -= Time.deltaTime;
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            var value = camera.CoreVirtualCamera.m_Lens.FieldOfView;
            camera.CoreVirtualCamera.m_Lens.FieldOfView = value - Time.deltaTime * 7;
            
            if (animationsComponent.Time <= 0)
                EndAnimations();
        }

        private void EndAnimations()
        {
            _dataWorld.Select<AnimationsStartArenaCameraComponent>()
                .SelectFirstEntity().Destroy();
        }

        public void Destroy()
        {
            AnimationsStartArenaCameraAction.StartAnimations -= StartAnimations;
            AnimationsStartArenaCameraAction.ReturnCamera -= ReturnCameraViewArena;
        }
    }
}