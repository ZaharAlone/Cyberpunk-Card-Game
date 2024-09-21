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

        [Header("Ability Destroy Card")]
        public LocalizedString HeaderDestroyOneCard;
        public LocalizedString HeaderDestroyManyCard;

        public LocalizedString DescrDestroyOneCard;
        public LocalizedString DescrDestroyManyCard;
        
        [Header("Ability Discard Card")]
        public LocalizedString HeaderDiscardOneCard;
        public LocalizedString HeaderDiscardManyCard;

        public LocalizedString DescrDiscardOneCard;
        public LocalizedString DescrDiscardManyCard;
    }
}