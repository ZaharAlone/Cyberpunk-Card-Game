using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using FMODUnity;

namespace CyberNet.Global.Sound
{
    [EcsSystem(typeof(GlobalModule))]
    public class SoundSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SoundAction.PlayMusic += PlayMusic;
            SoundAction.PlaySound += PlaySound;
        }
        
        public void Init()
        {
            StartMeta();
        }
        
        private void PlaySound(EventReference sound)
        {
            var audioEvent = RuntimeManager.CreateInstance(sound);
            audioEvent.start();
            audioEvent.release();
        }
        private void PlayMusic(EventReference sound)
        {
            var audioEvent = RuntimeManager.CreateInstance(sound);
            audioEvent.start();
        }
        private void StartMeta()
        {
            ref var soundData = ref _dataWorld.OneData<SoundData>().Sound;
            PlayMusic(soundData.BackgroundMusicMainMenu);
        }

        public void Destroy()
        {
            SoundAction.PlayMusic -= PlayMusic;
            SoundAction.PlaySound -= PlaySound;
        }
    }
}