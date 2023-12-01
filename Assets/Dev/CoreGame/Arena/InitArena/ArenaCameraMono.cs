using UnityEngine;

namespace CyberNet.Core.Arena
{
    public class ArenaCameraMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _mainCamera;
        [SerializeField]
        private GameObject _leftCamera;
        [SerializeField]
        private GameObject _rightCamera;

        public void SetLeftCamera()
        {
            _mainCamera.SetActive(false);
            _leftCamera.SetActive(true);
            _rightCamera.SetActive(false);
        }
        
        public void SetRightCamera()
        {
            _mainCamera.SetActive(false);
            _leftCamera.SetActive(false);
            _rightCamera.SetActive(true);
        }
        
        public void SetMainCamera()
        {
            _mainCamera.SetActive(true);
            _leftCamera.SetActive(false);
            _rightCamera.SetActive(false);
        }
    }
}