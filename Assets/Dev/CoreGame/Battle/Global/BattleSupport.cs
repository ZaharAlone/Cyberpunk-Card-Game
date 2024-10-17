using CyberNet.Global;

namespace CyberNet.Core.Battle
{
    public static class BattleSupport
    {
        public static bool ControlEntityIsAI(PlayerOrAI playerOrAI)
        {
            if (playerOrAI != PlayerOrAI.None && playerOrAI != PlayerOrAI.Player)
                return true;
            else
                return false;
        }
    }
}