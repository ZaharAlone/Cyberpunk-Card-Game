using CyberNet.Core.Arena.ArenaHUDUI;
using UnityEngine;
namespace CyberNet.Core.Arena
{
    [CreateAssetMenu(fileName = "ArenaConfigSO", menuName = "Scriptable Object/Board Game/Arena Config")]
    public class ArenaConfigSO : ScriptableObject
    {
        public ArenaContainerUICharacterMono ContainerAvatarUnitPrefab;

        [Header("Select zone view")]
        public GameObject SelectZoneGrenade;
    }
}