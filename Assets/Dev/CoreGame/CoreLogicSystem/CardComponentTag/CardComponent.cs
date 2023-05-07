using UnityEngine;

namespace CyberNet.Core
{
    public struct CardComponent
    {
        public string GUID;
        public PlayerEnum Player;
        public Canvas Canvas;
        public GameObject GO;
        public Transform Transform;
        public CardMono CardMono;
        public CardConfig Stats;

        public string Key;
        public CardNations Nations;
        public int CyberpsychosisCount;
        public int Price;

        public AbilityCard Ability_0;
        public AbilityCard Ability_1;
    }
}