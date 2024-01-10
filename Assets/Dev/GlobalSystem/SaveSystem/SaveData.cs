using System;

namespace CyberNet.SaveSystem
{
    [Serializable]
    public struct SaveData
    {
        public ProgressCompany ProgressCompany;
    }

    [Serializable]
    public enum ProgressCompany
    {
        None,
        Stage_2,
        Stage_3,
        Stage_4,
        Stage_5,
        Stage_6,
        Stage_7,
        Stage_8,
        Stage_9,
        Stage_10
    }
}