using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CyberNet.Global.Settings
{
    public class SoundSlider : MonoBehaviour, IMoveHandler, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
    {
        [SerializeField]
        private bool _isButton;
        [SerializeField]
        public EventReference _sfxSelectButton;
        [SerializeField]
        public EventReference _sfxSliderMove;
        [SerializeField]
        public EventReference _sfxClickButton;

        public void OnPointerEnter(PointerEventData eventData)
        {
            PlaySoundEffect(_sfxSelectButton);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isButton)
                PlaySoundEffect(_sfxClickButton);
        }

        public void OnSelect(BaseEventData eventData)
        {
            PlaySoundEffect(_sfxSelectButton);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            if (_isButton)
                PlaySoundEffect(_sfxClickButton);
        }

        public void OnMove(AxisEventData eventData)
        {
            if (eventData.moveVector.x != 0)
                PlaySoundEffect(_sfxSliderMove);
        }

        private void PlaySoundEffect(EventReference sound)
        {
            RuntimeManager.CreateInstance(sound).start();
        }
    }
}