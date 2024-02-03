using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core
{
    public struct CardComponent
    {
        public string GUID;
        public int PlayerID;
        public Canvas Canvas;
        public GameObject GO;
        public RectTransform RectTransform;
        public CardMono CardMono;
        public CardConfigJson Stats;

        public string Key;
        public CardNations Nations;
        public int DestroyPointCount;
        public int Price;

        public AbilityCardContainer Ability_0;
        public AbilityCardContainer Ability_1;
        public AbilityCardContainer Ability_2;
    }
}