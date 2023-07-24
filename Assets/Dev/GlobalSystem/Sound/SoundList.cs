using FMODUnity;
using UnityEngine;

namespace CyberNet.Core.Sound
{
    [CreateAssetMenu(fileName = "SoundList", menuName = "Scriptable Object/Sound List")]
    public class SoundList : ScriptableObject
    {
        [Header("Music")]
        public EventReference BackgroundMusicMainMenu;
        public EventReference BackgroundMusicBattle;

        [Header("Effect")]
        public EventReference AttackSound;
    }
}