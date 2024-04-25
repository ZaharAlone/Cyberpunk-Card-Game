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
            SoundAction.PlaySound += PlaySound;
            SoundAction.PlayMusic += PlayMusic;
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
            ref var soundData = ref _dataWorld.OneData<SoundData>();
            if (soundData.CurrentBackgroundMusic.isValid())
                soundData.CurrentBackgroundMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            
            var audioEvent = RuntimeManager.CreateInstance(sound);
            audioEvent.start();
            audioEvent.release();

            soundData.CurrentBackgroundMusic = audioEvent;
        }
        
        private void StartMeta()
        {
            ref var soundData = ref _dataWorld.OneData<SoundData>().Sound;
            PlayMusic(soundData.BackgroundMusicMainMenu);
        }

        public void Destroy()
        {
            SoundAction.PlaySound -= PlaySound;
            SoundAction.PlayMusic -= PlayMusic;
        }
    }
}