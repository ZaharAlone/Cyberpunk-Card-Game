using UnityEngine;

namespace BoardGame.Core
{
    public struct CardComponent
    {
        public GameObject GO;
        public Transform Transform;
        public CardStats Stats;

        public string Key;
        public CardNations Nations;
        public int Price;
        public CardType Type;

        public CurrencyCard Primary;
        public CurrencyCard Ally;
        public CurrencyCard Scrap;
    }
}