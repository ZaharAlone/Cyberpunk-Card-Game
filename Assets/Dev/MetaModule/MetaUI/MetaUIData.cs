using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Meta
{
    public struct MetaUIData
    {
        [FormerlySerializedAs("UI")]
        public GameObject UIGO;
        [FormerlySerializedAs("MainMenuUIMono")]
        public MetaUIMono MetaUIMono;
    }
}