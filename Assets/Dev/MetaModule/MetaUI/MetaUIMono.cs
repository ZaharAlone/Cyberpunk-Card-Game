using UnityEngine;
using CyberNet.Meta.DemoGame;
using CyberNet.Meta.SelectPlayersForGame;
using CyberNet.Meta.SettingsUI;

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