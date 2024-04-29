using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using FMODUnity;
using UnityEngine;

namespace CyberNet.Global.Sound
{
    [EcsSystem(typeof(GlobalModule))]
    public class SoundSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float coreAmbientTimeDuration = 113f;

        public void PreInit()
        {
            SoundAction.PlaySound += PlaySound;
            SoundAction.PlayMusic += PlayMusic;
            SoundAction.StartCoreMusic += StartCoreMusic;
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

        private void StartCoreMusic()
        {
            ref var soundData = ref _dataWorld.OneData<SoundData>();

            var weightDropAmbientSoundRain = soundData.WeightDropAmbientSoundRainInCore;
            var weightDropAmbientBaseSound = 1;

            var randomSelectAmbient = Random.Range(0, (weightDropAmbientBaseSound + weightDropAmbientSoundRain));

            if (randomSelectAmbient <= weightDropAmbientBaseSound)
            {
                PlayMusic(soundData.Sound.BackgroundAmbientMap);
                soundData.WeightDropAmbientSoundRainInCore += 1;
            }
            else
            {
                PlayMusic(soundData.Sound.BackgroundAmbientMapRain);
                soundData.WeightDropAmbientSoundRainInCore = 0;
            }
            
            _dataWorld.NewEntity().AddComponent(new TimeComponent
            {
                Time = coreAmbientTimeDuration,
                Action = () => StartCoreMusic(),
            });
        }

        public void Destroy()
        {
            SoundAction.PlaySound -= PlaySound;
            SoundAction.PlayMusic -= PlayMusic;
            SoundAction.StartCoreMusic -= StartCoreMusic;
        }
    }
}