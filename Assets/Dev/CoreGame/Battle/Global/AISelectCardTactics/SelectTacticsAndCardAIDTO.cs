namespace CyberNet.Core.Battle.TacticsMode
{
    /// <summary>
    /// Передаем выбранную карту и тактику AI
    /// </summary>
    public struct SelectTacticsAndCardAIDTO
    {
        public string GUIDCard;
        public string BattleTactics;
    }
}