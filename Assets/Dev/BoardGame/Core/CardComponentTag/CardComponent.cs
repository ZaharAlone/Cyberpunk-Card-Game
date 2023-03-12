using UnityEngine;

namespace BoardGame.Core
{
    public struct CardComponent
    {
        public string GUID;
        public Canvas Canvas;
        public GameObject GO;
        public Transform Transform;
        public CardMono CardMono;
        public CardStats Stats;

        public string Key;
        public CardNations Nations;
        public int CyberpsychosisCount;
        public int Price;

        public AbilityCard Ability_0;
        public AbilityCard Ability_1;
    }
}