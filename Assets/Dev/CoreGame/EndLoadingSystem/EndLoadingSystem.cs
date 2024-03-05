using CyberNet.Global.Sound;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Meta;

namespace CyberNet.Core
{
    [EcsSystem(typeof(CoreModule))]
    public class EndLoadingSystem : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            LoadingGameScreenAction.CloseLoadingGameScreen?.Invoke();

            var coreMusic = _dataWorld.OneData<SoundData>().Sound.BackgroundMusicMap;
            SoundAction.PlayMusic?.Invoke(coreMusic);
        }
    }
}