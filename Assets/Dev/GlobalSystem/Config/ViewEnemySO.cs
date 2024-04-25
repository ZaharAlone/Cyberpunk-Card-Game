using CyberNet.Core;
using I2.Loc;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Global.Config
{
    [CreateAssetMenu(fileName = "ViewEnemySO", menuName = "Scriptable Object/Board Game/View Enemy SO", order = 0)]
    public class ViewEnemySO : ScriptableObject
    {
        public LocalizedString PlayingCardHeader;
        public LocalizedString PurchaseCardHeader;
        public LocalizedString DiscardCardHeader;
        public LocalizedString DestroyCardHeader;
        
        public CardMono CardForEnemyTurnView;
    }
}