using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Sound
{
    [CreateAssetMenu(fileName = "SoundList", menuName = "Scriptable Object/Sound List")]
    public class SoundList : ScriptableObject
    {
        [Header("Music")]
        public EventReference BackgroundMusicMainMenu;
        public EventReference BackgroundMusicBattle;

        [Header("Effect Card")]
        public EventReference GetCardInHand;
        public EventReference AttackSound;
        public EventReference StartInteractiveCard;
        public EventReference CancelInteractiveCard;
        public EventReference SelectCard;
        
        [Header("Effect Map")]
        public EventReference SelectCurrentTargetInMap;
        public EventReference AddUnitInMap;
    }
}