using FMODUnity;
using UnityEngine;

namespace CyberNet.Global.Sound
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

        [Header("Dialog")]
        public EventReference PrintDialog;
    }
}