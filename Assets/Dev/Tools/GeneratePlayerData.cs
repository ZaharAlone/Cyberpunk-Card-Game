using System.Collections.Generic;
using System.Linq;
using CyberNet.Meta;

namespace CyberNet.Tools
{
    public static class GeneratePlayerData
    {
        public static string GenerateUniquePlayerName(List<string> namesList, List<SelectLeaderData> selectLeaders)
        {
            var random = new System.Random();

            var botName = "";
            var isUniqueName = false;
            while (!isUniqueName)
            {
                botName = namesList.ElementAt(random.Next(namesList.Count));
                isUniqueName = true;
                foreach (var leader in selectLeaders)
                {
                    if (leader.NamePlayer == botName)
                    {
                        isUniqueName = false;
                    }
                }
            }

            return botName;
        }
        
        public static List<string> GetRandomLeader(Dictionary<string, LeadersConfig> leadersConfig, int count)
        {
            var nameLeaders = new List<string>();

            while (nameLeaders.Count != count)
            {
                var random = new System.Random();
                var selectLeader = leadersConfig.ElementAt(random.Next(leadersConfig.Count));

                var isUseLeader = false;
                foreach (var useLeader in nameLeaders)
                {
                    if (useLeader == selectLeader.Key)
                    {
                        isUseLeader = true;
                    }
                }
                
                if (!isUseLeader)
                    nameLeaders.Add(selectLeader.Key);
            }
            
            return nameLeaders;
        }
    }
}