using System.Collections.Generic;
using Animancer;
using DG.Tweening;
using UnityEngine;

namespace CyberNet.Meta
{
    public class LoadingGameScreenUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject Background;
        [SerializeField]
        private GameObject Panel;
        [SerializeField]
        private AnimancerComponent Animancer;
        [SerializeField]
        private AnimationClip LoadingAnimation;
        
        public void OpenWindow()
        {
            Background.SetActive(true);    
            Panel.SetActive(true);
            Animancer.Play(LoadingAnimation);
        }
        
        public void CloseWindow()
        {
            Background.SetActive(false);    
            Panel.SetActive(false);    
            Animancer.Stop();
        }
    }
}