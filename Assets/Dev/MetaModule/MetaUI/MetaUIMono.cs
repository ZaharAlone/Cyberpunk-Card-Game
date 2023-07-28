using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace CyberNet.Meta
{
    public class MetaUIMono : MonoBehaviour
    {
        public MainMenuUIMono MainMenuUIMono;
        public CampaignUIMono CampaignUIMono;
        public SelectLeadersUIMono SelectLeadersUIMono;
        public OnlineGameUIMono OnlineGameUIMono;
        [FormerlySerializedAs("VSScreenUIMono")]
        public LoadingVSScreenUIMono loadingVSScreenUIMono;
    }
}