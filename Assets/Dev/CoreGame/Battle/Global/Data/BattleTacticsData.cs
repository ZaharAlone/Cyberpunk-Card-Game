using System.Collections.Generic;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;

namespace CyberNet.Core.Battle.TacticsMode
{
    public struct BattleTacticsData
    {
        public List<BattleTactics> BattleTactics;

        public InteractiveCardTacticsScreenMono CardFoTacticsScreen;
        public string KeyNeutralBattleCard;
    }
}