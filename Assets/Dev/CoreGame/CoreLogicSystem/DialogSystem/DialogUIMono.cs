using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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
        private TextMeshProUGUI _textDialog;

        [SerializeField]
        private GameObject _textToContinue;

        public void OnEnable()
        {
            CloseDialog();
        }

        public void OnClickNextDialog()
        {
            DialogAction.ClickContinueButton?.Invoke();
        }

        public void SetViewDialog(Sprite avatar, string name)
        {
            _avatar.sprite = avatar;
            _name.Term = name;
        }

        public void SetDialogText(string textDialog)
        {
            _textDialog.text = textDialog;
        }

        public void OpenDialog()
        {
            _background.SetActive(true);
            _panel.SetActive(true);
        }

        public void SetEnableTextToContinue(bool status)
        {
            _textToContinue.SetActive(status);
        }

        public void CloseDialog()
        {
            _background.SetActive(false);
            _panel.SetActive(false);
        }
    }
}