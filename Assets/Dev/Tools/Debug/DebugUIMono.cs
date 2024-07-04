using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Tools.DebugGame
{
    public class DebugUIMono : MonoBehaviour
    {
        [SerializeField]
        [Required]
        private GameObject _panel;
        [SerializeField]
        private GameObject _debugButton;
        [SerializeField]
        private TMP_Dropdown _dropdown;

        private string logInfo;
        private List<string> _listCardGame;
        private int _selectCard;

        public void OnEnable()
        {
            _panel.SetActive(false);
        }

        public void OnClickOpenDebug()
        {
            _panel.SetActive(true);
        }

        public void OnClickCloseDebug()
        {
            _panel.SetActive(false);
        }

        public void SetListCardGame(List<string> listCard)
        {
            _listCardGame = listCard;
            _dropdown.AddOptions(listCard);
        }
        
        public void EnterNameCard(int selectCardID)
        {
            _selectCard = selectCardID;
        }

        public void OnClickGetCard()
        {
            DebugAction.GetCard?.Invoke(_listCardGame[_selectCard]);
        }

        public void OnClickHideDebug()
        {
            _panel.SetActive(false);
            _debugButton.SetActive(false);
        }
    }
}