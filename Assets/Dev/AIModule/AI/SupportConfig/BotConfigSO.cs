using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.Enemy
{
    [CreateAssetMenu(fileName = "BotConfig", menuName = "Scriptable Object/Board Game/Bot Config", order = 0)]
    public class BotConfigSO : ScriptableObject
    {
        public TextAsset BotAIConfigJson;
    }
}