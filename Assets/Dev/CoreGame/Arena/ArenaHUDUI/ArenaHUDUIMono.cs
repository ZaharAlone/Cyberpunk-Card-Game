using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Arena.ArenaHUDUI
{
    public class ArenaHUDUIMono : MonoBehaviour
    {
        public Transform ContainerListCharacter;

        [SerializeField]
        private GameObject _actionAttackButton;
        [SerializeField]
        private GameObject _actionRetreatButton;
        
        [SerializeField]
        private List<ArenaContainerUICharacterMono> _characterAvatars = new List<ArenaContainerUICharacterMono>();

        private void OnEnable()
        {
            HideArenaUI();
        }

        public void OnArenaHUD()
        {
            ContainerListCharacter.gameObject.SetActive(true);
            _characterAvatars = new List<ArenaContainerUICharacterMono>();
        }
        
        public void OffArenaHUD()
        {
            ClearCharacters();
            ContainerListCharacter.gameObject.SetActive(false);
        }

        public void AddCharacterAvatars(ArenaContainerUICharacterMono characterMono)
        {
            Debug.LogError($"Add character avatars {characterMono.name}");
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

        public void ShowArenaUI(bool showRetreatButton = true)
        {
            _actionAttackButton.SetActive(true);
            _actionRetreatButton.SetActive(showRetreatButton);
        }
        
        public void HideArenaUI()
        {
            _actionAttackButton.SetActive(false);
            _actionRetreatButton.SetActive(false);
        }

        public void OnClickAttack()
        {
            ArenaUIAction.ClickAttack?.Invoke();
        }

        public void OnClickRetreat()
        {
            ArenaUIAction.ClickRetreat?.Invoke();
        }

        private void ClearCharacters()
        {
            foreach (var character in _characterAvatars)
            {
                Destroy(character.gameObject);
            }
            
            _characterAvatars.Clear();
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