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
        public GameObject Background;
        public GameObject Panel;

        [Header("Content WizardLegacy.Core.Dialog")]
        public Image Avatar;
        public Localize Name;
        public Localize TextDialog;

        public void OnClickNextDialog()
        {
            DialogAction.NextDialog?.Invoke();
        }

        public void SetViewDialog(Sprite avatar, string name, string textDialog)
        {
            Avatar.sprite = avatar;
            Name.Term = name;
            TextDialog.Term = textDialog;
        }

        public void OpenDialog()
        {
            Background.SetActive(true);
            Panel.SetActive(true);
        }

        public void CloseDialog()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }
    }
}