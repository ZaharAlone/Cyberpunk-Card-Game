using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CyberNet.Core.Dialog
{
    public static class DialogAction
    {
        public static Action<string> StartDialog;
        public static Action ClickContinueButton;
        public static Action EndDialog;
    }
}