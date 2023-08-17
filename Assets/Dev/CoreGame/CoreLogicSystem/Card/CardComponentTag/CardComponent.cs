using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core
{
    public struct CardComponent
    {
        public string GUID;
        public PlayerEnum Player;
        public Canvas Canvas;
        public GameObject GO;
        public RectTransform RectTransform;
        public CardMono CardMono;
        public CardConfigJson Stats;

        public string Key;
        public CardNations Nations;
        public int DestroyPointCount;
        public int Price;

        public AbilityCard Ability_0;
        public AbilityCard Ability_1;
        public AbilityCard Ability_2;
    }
}