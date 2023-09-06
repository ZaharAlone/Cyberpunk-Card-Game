using System;
using System.Collections.Generic;
using UnityEngine;
namespace CyberNet.Meta.SelectPlayersForGame
{
    public class SelectPlayersUIMono : MonoBehaviour
    {
        [Header("Global UI")]
        public GameObject Background;
        public GameObject Panel;
        
        public List<SelectPlayerSlotUIMono> SelectPlayerSlot = new();

        public void Awake()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }

        public void OpenWindow()
        {
            Background.SetActive(true);
            Panel.SetActive(true);
        }

        public void CloseWindow()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }

        public void OnClickBack()
        {
            SelectPlayerAction.OnClickBack?.Invoke();
        }

        public void OnClickStartGame()
        {
            SelectPlayerAction.OnClickStartGame?.Invoke();
        }
    }
}