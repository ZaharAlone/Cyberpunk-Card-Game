using EcsCore;
using Cinemachine;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Global.GameCamera
{
    [EcsSystem(typeof(GlobalModule))]
    public class GameCameraInitSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ref var camera = ref _dataWorld.OneData<GameCameraData>();
            camera.GameCameraMono = camera.CameraGO.GetComponent<GameCameraMono>();
            camera.MainCamera = camera.GameCameraMono.MainCamera;
            camera.CoreVirtualCamera = camera.GameCameraMono.CoreVirtualCamera;
            camera.CoreCinemachineTransposer = camera.CoreVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            camera.FollowOffsetCoreCamera = camera.CoreCinemachineTransposer.m_FollowOffset;
            camera.MetaVirtualCamera = camera.GameCameraMono.MetaVirtualCamera;
            
            camera.CoreVirtualCamera.gameObject.SetActive(false);
        }
    }
}