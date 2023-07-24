using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Sound;
using FMODUnity;

namespace CyberNet.Global.Sound
{
    [EcsSystem(typeof(GlobalModule))]
    public class SoundSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            SoundAction.PlayMusic += PlayMusic;
            SoundAction.PlaySound += PlaySound;
            
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
    }
}