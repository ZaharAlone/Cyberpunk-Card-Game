using UnityEngine;

namespace CyberNet.Core.AI
{
    [CreateAssetMenu(fileName = "BotConfig", menuName = "Scriptable Object/Board Game/Bot Config", order = 0)]
    public class BotConfigSO : ScriptableObject
    {
        [Header("Json")]
        public TextAsset BotAIConfigJson;
        public TextAsset BotNameConfig;
    }
}