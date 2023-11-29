using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Arena.ArenaHUDUI
{
    public class ArenaHUDUIMono : MonoBehaviour
    {
        public Transform ContainerListCharacter;
        
        private List<ArenaContainerUICharacterMono> _characterAvatars = new List<ArenaContainerUICharacterMono>();
        
        public void OnArenaHUD()
        {
            ContainerListCharacter.gameObject.SetActive(true);
        }
        
        public void OffArenaHUD()
        {
            ContainerListCharacter.gameObject.SetActive(false);
        }

        public void AddCharacterAvatars(ArenaContainerUICharacterMono characterMono)
        {
            _characterAvatars.Add(characterMono);
        }

        public void UpdateOrderAvatarSlot()
        {
            var currentIndexPosition = 0;
            var listSlot = _characterAvatars;

            while (listSlot.Count != 0)
            {
                var minIndex = 999;
                var selectSlot = 0;

                for (int i = 0; i < listSlot.Count; i++)
                {
                    if (listSlot[i].PositionInTurnQueue < minIndex)
                    {
                        minIndex = listSlot[i].PositionInTurnQueue;
                        selectSlot = i;
                    }
                }
                
                listSlot[selectSlot].transform.SetSiblingIndex(currentIndexPosition);
                if (currentIndexPosition == 0)
                    listSlot[selectSlot].OnCurrentCharacter();
                
                currentIndexPosition++;
                listSlot.RemoveAt(selectSlot);
            }
        }

        [Button]
        public void NextRoundTest()
        {
            NextRoundUpdateViewAvatar();
        }
        
        public void NextRoundUpdateViewAvatar()
        {
            ContainerListCharacter.GetChild(0).SetAsLastSibling();
            
        }
    }
}