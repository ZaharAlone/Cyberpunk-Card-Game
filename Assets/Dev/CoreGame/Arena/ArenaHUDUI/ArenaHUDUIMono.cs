using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.Arena.ArenaHUDUI
{
    public class ArenaHUDUIMono : MonoBehaviour
    {
        [SerializeField]
        private Transform _containerListCharacter;

        [SerializeField]
        private GameObject _actionRetreatButton;
        
        [Header("Scale character avatar")]
        [SerializeField]
        private float _scaleAvatarBase = 100;
        [SerializeField]
        private float _scaleAvatarSelect = 120;
        [SerializeField]
        private float _spacingBetweenAvatar = 10;

        private List<ArenaContainerUICharacterMono> _characterAvatars = new List<ArenaContainerUICharacterMono>();
        
        private void OnEnable()
        {
            HideArenaUI();
        }

        public ArenaContainerUICharacterMono AddAvatarSlotInBar(ArenaContainerUICharacterMono arenaCharacter)
        {
            var avatarSlot = Instantiate(arenaCharacter, _containerListCharacter);
            _characterAvatars.Add(avatarSlot);
            return avatarSlot;
        }

        public void OnArenaHUD()
        {
            _containerListCharacter.gameObject.SetActive(true);
        }
        
        public void OffArenaHUD()
        {
            ClearCharacters();
            _containerListCharacter.gameObject.SetActive(false);
        }

        public void SetStartPositionAvatar()
        {
            foreach (var avatar in _characterAvatars)
            {
                var positionAndScale = CalculatePositionsAndScaleAvatar(avatar);
                avatar.SetPositionAndScale(new Vector2(positionAndScale.Item1, 0), positionAndScale.Item2);
            }
        }

        private (float, float) CalculatePositionsAndScaleAvatar(ArenaContainerUICharacterMono avatar)
        {
            var countAvatarNotMain = _characterAvatars.Count - 1;
            var maxLengthSpaceAvatar = (_scaleAvatarBase + _spacingBetweenAvatar) * countAvatarNotMain + _scaleAvatarSelect;
            var startPosition = -maxLengthSpaceAvatar / 2;

            var positionX = startPosition;
            var scale = 0f;
            
            if (avatar.PositionInTurnQueue == 0)
            {
                scale = _scaleAvatarSelect;
            }
            else
            {
                positionX = startPosition + (_scaleAvatarSelect + _spacingBetweenAvatar)
                    + (_spacingBetweenAvatar + _scaleAvatarBase) * (avatar.PositionInTurnQueue - 1);
                scale = _scaleAvatarBase;
            }
            
            return (positionX, scale);
        }

        public void UpdateOrderAvatarSlot()
        {
            foreach (var avatar in _characterAvatars)
            {
                avatar.PositionInTurnQueue--;
                if (avatar.PositionInTurnQueue < 0)
                    avatar.PositionInTurnQueue = _characterAvatars.Count - 1;
            }
            
            foreach (var avatar in _characterAvatars)
            {
                var positionAndScale = CalculatePositionsAndScaleAvatar(avatar);

                if (avatar.PositionInTurnQueue == _characterAvatars.Count - 1)
                {
                    avatar.AnimationsMoveBack(positionAndScale.Item1, positionAndScale.Item2);
                }
                else
                {
                    avatar.AnimationsMoveNext(positionAndScale.Item1, positionAndScale.Item2);
                }
            }
        }

        public void ShowArenaUI(bool showRetreatButton = true)
        {
            _actionRetreatButton.SetActive(showRetreatButton);
        }
        
        public void HideArenaUI()
        {
            _actionRetreatButton.SetActive(false);
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
            _containerListCharacter.GetChild(0).SetAsLastSibling();
            
        }
    }
}