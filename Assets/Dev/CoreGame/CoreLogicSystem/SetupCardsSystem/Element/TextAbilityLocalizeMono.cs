using I2.Loc;
using TMPro;
using UnityEngine;

namespace CyberNet.Core.UI
{
    public class TextAbilityLocalizeMono : MonoBehaviour
    {
        [SerializeField]
        private Localize _text;
        [SerializeField]
        private LocalizationParamsManager _localizeParameters;

        public void SetText(string text)
        {
            _text.Term = text;
        }

        public void SetParameters(int value)
        {
            _localizeParameters.SetParameterValue("count", value.ToString());
        }
    }
}