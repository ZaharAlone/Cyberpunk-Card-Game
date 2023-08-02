namespace CyberNet.Tools
{
    public static class RevertPlayer
    {
        public static PlayerEnum Revert(PlayerEnum target)
        {
            if (target == PlayerEnum.Player1)
                return PlayerEnum.Player2;
            else
                return PlayerEnum.Player1;
        }
    }
}