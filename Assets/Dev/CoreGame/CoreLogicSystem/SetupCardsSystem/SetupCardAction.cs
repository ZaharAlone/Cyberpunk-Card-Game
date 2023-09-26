using System;
using ModulesFramework.Data;
using UnityEngine;

namespace CyberNet.Core
{
    public static class SetupCardAction
    {
        public static Func<CardData, Transform, bool, Entity> InitCard;
    }
}