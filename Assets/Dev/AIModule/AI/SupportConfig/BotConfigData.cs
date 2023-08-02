using System.Collections.Generic;

namespace CyberNet.Core.Enemy
{
    public struct BotConfigData
    {
        public Dictionary<string, float> BotScoreCard;
        public List<string> BotNameConfig;

        public BotConfigSO BotConfigSO;
    }
}