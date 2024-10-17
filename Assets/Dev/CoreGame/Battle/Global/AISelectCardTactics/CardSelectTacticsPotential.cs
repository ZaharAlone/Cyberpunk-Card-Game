namespace CyberNet.Core.Battle.TacticsMode
{
    public struct CardSelectTacticsPotential
    {
        public string GUID;

        public BattleTactics SelectTactics;
        
        public int Power;
        public int Kill;
        public int Defence;

        public float EfficiencyPower;
        public float EfficiencyKillDefence;
    }
}