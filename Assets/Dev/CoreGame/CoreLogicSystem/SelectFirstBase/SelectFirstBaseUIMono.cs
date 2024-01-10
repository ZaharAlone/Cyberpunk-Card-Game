using System;
using UnityEngine;

namespace CyberNet.Core.SelectFirstBase
{
    public class SelectFirstBaseUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject Panel;

        public void Awake()
        {
            Panel.SetActive(false);
        }

        public void OpenWindow()
        {
            Panel.SetActive(true);
        }

        public void CloseWindow()
        {
            Panel.SetActive(false);
        }
    }
}