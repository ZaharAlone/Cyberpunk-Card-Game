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
        public int Price;
        public CardType Type;

        public AbilityCard Ability;
        public AbilityCard FractionsAbility;
        public AbilityCard DropAbility;
    }
}