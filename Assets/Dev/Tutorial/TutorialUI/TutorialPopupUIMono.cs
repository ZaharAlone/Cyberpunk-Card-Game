using UnityEngine;

namespace CyberNet.Tutorial.UI
{
    public class TutorialPopupUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _zoomCamera_keyboard;
        [SerializeField]
        private GameObject _zoomCamera_gamepad;
        [SerializeField]
        private GameObject _moveCamera_keyboard;
        [SerializeField]
        private GameObject _moveCamera_gamepad;

        public void OpenZoomCamera(bool isGamepad)
        {
            if (isGamepad)
            {
                _zoomCamera_gamepad.SetActive(true);
            }
            else
            {
                _zoomCamera_keyboard.SetActive(true);
            }
        }

        public void OpenMoveCamera(bool isGamepad)
        {
            if (isGamepad)
            {
                _moveCamera_gamepad.SetActive(true);
            }
            else
            {
                _moveCamera_keyboard.SetActive(true);
            }
        }

        public void CloseAllPopup()
        {
            //_zoomCamera_gamepad.SetActive(false);
            _zoomCamera_keyboard.SetActive(false);
            //_moveCamera_gamepad.SetActive(false);
            _moveCamera_keyboard.SetActive(false);
        }
    }
}