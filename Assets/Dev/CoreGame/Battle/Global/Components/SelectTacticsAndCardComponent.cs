namespace CyberNet.Core.Battle.TacticsMode
{
    /// <summary>
    /// Передаем выбранную карту и тактику AI
    /// </summary>
    public struct SelectTacticsAndCardComponent
    {
        public string GUIDCard;
        public string KeyCard;
        public string BattleTactics;
    }
}