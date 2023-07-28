using System.Collections.Generic;
using UnityEngine.Serialization;
namespace CyberNet.Core.Dialog
{
    public struct DialogConfigData
    {
        public DialogConfigSO DialogConfigSO;
        public Dictionary<string, DialogConfig> DialogConfig;
        public Dictionary<string, CharacterDialogConfig> CharacterDialogConfig;
    }
}