using System.Collections.Generic;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.Battle.TacticsMode
{
    [CreateAssetMenu(fileName = "BattleTacticsConfig", menuName = "Scriptable Object/Battle/Battle Tactics Config", order = 0)]
    public class BattleTacticsSO : SerializedScriptableObject
    {
        public List<BattleTactics> BattleTactics;

        public InteractiveCardTacticsScreenMono CardFoTacticsScreen;
        public string KeyNeutralBattleCard = "neutral_battle";
    }
}