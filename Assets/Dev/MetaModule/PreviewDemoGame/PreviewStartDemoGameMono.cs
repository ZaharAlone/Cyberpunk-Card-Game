using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Meta.DemoGame
{
    public class PreviewStartDemoGameMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _background;
        [SerializeField]
        private GameObject _panel;

        public void Awake()
        {
            _background.SetActive(false);
            _panel.SetActive(false);
        }

        public void OpenWindow()
        {
            _background.SetActive(true);
            _panel.SetActive(true);
        }
        
        public void CloseWindow()
        {
            _background.SetActive(false);
            _panel.SetActive(false);
        }

        public void OnClickClose()
        {
            CloseWindow();
        }
    }
}