using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Global.Sound
{
    [CreateAssetMenu(fileName = "SoundList", menuName = "Scriptable Object/Sound List")]
    public class SoundList : ScriptableObject
    {
        [Header("Music")]
        public EventReference BackgroundMusicMainMenu;
        public EventReference BackgroundMusicMap;
        public EventReference BackgroundMusicBattleArena;

        [Header("UI")]
        public EventReference ButtonClick;
        public EventReference DialogNextPhrase;
        
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