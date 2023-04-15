namespace BoardGame.Core
{
    /// <summary>
    /// Сколько карт выдать и кому
    /// </summary>
    public struct EventDistributionCard
    {
        public PlayerEnum Target;
        public int Count;
    }
}