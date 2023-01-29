using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MainMenuMono : MonoBehaviour
{
    public static Action ButtonStartGame;
    public static Action ButtonConnectServer;

    public void StartGame()
    {
        ButtonStartGame?.Invoke();
    }

    public void ConnectServer()
    {
        ButtonConnectServer?.Invoke();
    }
}
