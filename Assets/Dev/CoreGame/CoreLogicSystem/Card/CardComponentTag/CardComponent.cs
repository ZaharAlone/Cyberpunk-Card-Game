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

        public int ValueLeftPoint;
        public int ValueRightPoint;
        
        public string Key;
        public int Price;

        public AbilityCardContainer Ability_0;
        public AbilityCardContainer Ability_1;
    }
}