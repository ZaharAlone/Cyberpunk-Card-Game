using I2.Loc;
using UnityEngine;

namespace CyberNet.Global.Config
{
    [CreateAssetMenu(fileName = "SupportLocalizeSO", menuName = "Scriptable Object/Board Game/Support Localize")]
    public class SupportLocalizeSO : ScriptableObject
    {
        [Header("Localize Ability Input")]
        public LocalizedString HeaderTakeDamage;

        public LocalizedString CancelButton;
        public LocalizedString DiscardCard;
        public LocalizedString TakeDamage;
    }
}