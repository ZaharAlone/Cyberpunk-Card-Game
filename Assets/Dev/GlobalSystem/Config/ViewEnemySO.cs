using CyberNet.Core;
using I2.Loc;
using UnityEngine;

namespace CyberNet.Global.Config
{
    [CreateAssetMenu(fileName = "ViewEnemySO", menuName = "Scriptable Object/Board Game/View Enemy SO", order = 0)]
    public class ViewEnemySO : ScriptableObject
    {
        public LocalizedString PlayingCardHeader;
        public LocalizedString BuyCardHeader;
        public CardMono CardForEnemyTurnView;
    }
}