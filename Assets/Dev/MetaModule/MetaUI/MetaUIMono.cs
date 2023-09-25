using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CyberNet.Meta.DemoGame;
using CyberNet.Meta.SelectPlayersForGame;
using UnityEngine.Serialization;

namespace CyberNet.Meta
{
    public class MetaUIMono : MonoBehaviour
    {
        public MainMenuUIMono MainMenuUIMono;
        public CampaignUIMono CampaignUIMono;
        public SelectLeadersUIMono SelectLeadersUIMono;
        public SelectPlayersUIMono SelectPlayersUIMono;
        public OnlineGameUIMono OnlineGameUIMono;
        public LoadingGameScreenUIMono loadingGameScreenUIMono;

        [Header("Demo Game")]
        public PreviewStartDemoGameMono PreviewStartDemoGameMono;
        public PreviewEndDemoGameMono PreviewEndDemoGameMono;
    }
}