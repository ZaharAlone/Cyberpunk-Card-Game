using FMODUnity;
using UnityEngine;

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

        [Header("UI Core")]
        public EventReference StartTurnPlayer;
        public EventReference EndTurnPlayer;
        public EventReference StartTurnEnemy;
        public EventReference EndTurnEnemy;
        
        [Header("Effect Card")]
        public EventReference GetCardInHand;
        public EventReference AttackSound;
        public EventReference StartInteractiveCard;
        public EventReference CancelInteractiveCard;
        public EventReference SelectCard;
        public EventReference FlipCard;
        
        [Header("Effect Map")]
        public EventReference SelectCurrentTargetInMap;
        public EventReference MoveArrowMap;
        public EventReference AddUnitInMap;
        public EventReference KillUnitMap;
        public EventReference SelectUnitInMap;
        public EventReference DeselectUnitInMap;

        [Header("Dialog")]
        public EventReference PrintDialog;

        [Header("Arena")]
        public EventReference AimGun;
        public EventReference Shoot;
        public EventReference HitUnit;
        public EventReference HitShield;
    }
}