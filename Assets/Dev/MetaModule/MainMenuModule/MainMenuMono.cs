using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuMono : MonoBehaviour
{
    public GameObject MainMenuUIBlock;
    public GameObject SelectTypeOnlineGameUIBlock;

    public static Action ButtonStartGame;
    public static Action ButtonConnectServer;

    public void PlayOnline()
    {
        MainMenuUIBlock.SetActive(false);
        SelectTypeOnlineGameUIBlock.SetActive(true);
    }

    public void SearchOnlineGame()
    {
        ButtonConnectServer?.Invoke();
    }

    public void PlayCampaign()
    {

    }

    public void PlayVsAI()
    {
        ButtonStartGame?.Invoke();
    }

    public void PlayPassAndPlay()
    {

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
