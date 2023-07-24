using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CyberNet.Core
{
    public static class SortingDeckCardAnimationsAction
    {
        public static Func<PlayerEnum, float> GetTimeSortingDeck;
        public static Func<PlayerEnum, float> GetTimeCardToHand;
    }
}