namespace CyberNet.Core.Arena
{
    public struct ArenaData
    {
        public ArenaMono ArenaMono;

        // Используется для отображения вьюхи на арене, если сражаются боты,
        // мы бой не показываем, а показываем только результат.
        public bool IsShowVisualBattle;
    }
}