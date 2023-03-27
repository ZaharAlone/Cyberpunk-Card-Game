using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BoardGame.Meta
{
    public class MainMenuMono : MonoBehaviour
    {
        public GameObject MainMenuUIBlock;
        public GameObject SelectTypeOnlineGameUIBlock;

        public void PlayOnline()
        {
            MainMenuUIBlock.SetActive(false);
            SelectTypeOnlineGameUIBlock.SetActive(true);
        }

        public void SearchOnlineGame()
        {
            MainMenuAction.ConnectServer?.Invoke();
        }

        public void PlayCampaign()
        {

        }

        public void PlayVsAI()
        {
            MainMenuAction.StartGame?.Invoke();
        }

        public void PlayPassAndPlay()
        {
            MainMenuAction.StartGamePassAndPlay?.Invoke();
        }

        public void Settings()
        {

        }

        public void Exti()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
		        Application.Quit();
#endif
        }

        public void GoToWebPage(string url)
        {
            Application.OpenURL(url);
        }
    }
}