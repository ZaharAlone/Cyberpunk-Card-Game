using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using UnityEngine.Serialization;

namespace CyberNet.Core.Dialog
{
    public class DialogUIMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _background;
        [SerializeField]
        private GameObject _panel;

        [Header("Content Dialog")]
        [SerializeField]
        private Image _avatar;
        [SerializeField]
        private Localize _name;
        [SerializeField]
        private Localize _textDialog;

        public void OnEnable()
        {
            CloseDialog();
        }

        public void OnClickNextDialog()
        {
            Debug.LogError("OnClick next dialog");
            DialogAction.NextDialog?.Invoke();
        }

        public void SetViewDialog(Sprite avatar, string name, string textDialog)
        {
            _avatar.sprite = avatar;
            _name.Term = name;
            _textDialog.Term = textDialog;
        }

        public void OpenDialog()
        {
            _background.SetActive(true);
            _panel.SetActive(true);
        }

        public void CloseDialog()
        {
            _background.SetActive(false);
            _panel.SetActive(false);
        }
    }
}