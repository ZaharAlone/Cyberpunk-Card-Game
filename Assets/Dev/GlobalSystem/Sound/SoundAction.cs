using System;
using FMODUnity;
namespace CyberNet.Global.Sound
{
    public static class SoundAction
    {
        public static Action<EventReference> PlaySound;
        public static Action<EventReference> PlayMusic;
        public static Action StartCoreMusic;
        public static Action StartLoadingCore;
    }
}