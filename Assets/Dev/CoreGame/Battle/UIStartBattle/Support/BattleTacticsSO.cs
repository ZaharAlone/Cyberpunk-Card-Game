using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.Battle.TacticsMode
{
    [CreateAssetMenu(fileName = "BattleTacticsConfig", menuName = "Scriptable Object/Battle/Battle Tactics Config", order = 0)]
    public class BattleTacticsSO : SerializedScriptableObject
    {
        public List<BattleTactics> BattleTactics;
    }
}