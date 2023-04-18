using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BoardGame.Core
{
    public static class WaitCardAnimationsAction
    {
        public static Func<PlayerEnum, float> GetTimeSortingDeck;
        public static Func<PlayerEnum, float> GetTimeCardToHand;
    }
}