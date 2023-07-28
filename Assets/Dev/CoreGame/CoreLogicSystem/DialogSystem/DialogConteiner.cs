using System;
using I2.Loc;

namespace CyberNet.Core.Dialog
{
    [Serializable]
    public struct DialogConteiner
    {
        public string AvatarKey;
        public LocalizedString NameSpeaker;
        public LocalizedString DialogText;
    }
}