using System.Collections.Generic;
using System.Linq;
using CyberNet.Meta;

namespace CyberNet.Tools
{
    public static class GenerateUniqueBotName
    {
        public static string Generate(List<string> namesList, List<SelectLeaderData> selectLeaders)
        {
            var random = new System.Random();

            var botName = "";
            var isUniqueName = false;
            while (!isUniqueName)
            {
                botName = namesList.ElementAt(random.Next(namesList.Count));   
                
                foreach (var leader in selectLeaders)
                {
                    if (leader.NamePlayer != botName)
                    {
                        isUniqueName = true;
                    }
                }
            }

            return botName;
        }
    }
}