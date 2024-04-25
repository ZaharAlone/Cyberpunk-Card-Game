using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Threading.Tasks;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.EnemyTurnView
{
    [EcsSystem(typeof(CoreModule))]
    public class EnemyTurnViewUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        private const float wait_time_between_animations_seconds = 0.3f;
        private const int wait_time_end_animations = 500;
        private const float automatic_close_ui_time = 2f;

        public void PreInit()
        {
            EnemyTurnViewUIAction.ShowViewEnemyCard += ShowViewCard;
            EnemyTurnViewUIAction.ForceHideView += HideView;
        }

        private void ShowViewCard(EnemyTurnActionType typeView, string keyCard)
        {
            var isShowPanel = _dataWorld.Select<EnemyTurnViewUIComponent>()
                .TrySelectFirstEntity(out var enemyViewCardEntity);

            var isViewNewCardDelay = false;
            
            if (!isShowPanel)
            {
                OpenView(typeView);

                var enemyNewViewCardEntity = _dataWorld.NewEntity()
                    .AddComponent(new EnemyTurnViewUIComponent {
                        CurrentTurn = typeView,
                    });

                enemyNewViewCardEntity.AddComponent(new TimeComponent {
                    Time = automatic_close_ui_time,
                    Action = () => HideView(),
                });

                isViewNewCardDelay = true;
            }
            else
            {
                var enemyViewCardComponent = enemyViewCardEntity.GetComponent<EnemyTurnViewUIComponent>();

                if (typeView != enemyViewCardComponent.CurrentTurn)
                {
                    SwitchCurrentViewToNewType(typeView);
                    isViewNewCardDelay = true;
                }
            }

            if (isViewNewCardDelay)
            {
                _dataWorld.NewEntity().AddComponent(new TimeComponent {
                    Time = wait_time_between_animations_seconds,
                    Action = () => AddNewCardView(keyCard),
                });
            }
            else
            {
                AddNewCardView(keyCard);   
            }
        }

        private void OpenView(EnemyTurnActionType typeView)
        {
            var turnUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PlayerEnemyTurnActionUIMono;
            var currentPlayerEntity = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();
            
            var playerViewComponent = currentPlayerEntity.GetComponent<PlayerViewComponent>();
            ref var cityVisual = ref _dataWorld.OneData<BoardGameData>().CitySO;
            cityVisual.UnitDictionary.TryGetValue(playerViewComponent.KeyCityVisual, out var playerUnitVisual);
            
            var header = SelectHeaderPanel(typeView);
            
            turnUI.SetViewPlayer(playerViewComponent.Avatar, playerViewComponent.Name, 
                playerUnitVisual.IconsUnit, playerUnitVisual.ColorUnit);
            turnUI.SetHeaderView(header);
            
            turnUI.EnableFrame();
        }

        private string SelectHeaderPanel(EnemyTurnActionType typeView)
        {
            var viewEnemyConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ViewEnemySO;
            var header = "";

            switch (typeView)
            {
                case EnemyTurnActionType.PlayingCard:
                    header = viewEnemyConfig.PlayingCardHeader;
                    break;
                case EnemyTurnActionType.PurchaseCard:
                    header = viewEnemyConfig.PurchaseCardHeader;
                    break;
                case EnemyTurnActionType.DiscardCard:
                    header = viewEnemyConfig.DiscardCardHeader;
                    break;
                case EnemyTurnActionType.DestroyCard:
                    header = viewEnemyConfig.DestroyCardHeader;
                    break;
            }
            
            return header;
        }

        private void SwitchCurrentViewToNewType(EnemyTurnActionType typeView)
        {
            var turnUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PlayerEnemyTurnActionUIMono;
            turnUI.ClearContainerCard();

            var header = SelectHeaderPanel(typeView);
            turnUI.SetHeaderView(header);
        }
        
        private void AddNewCardView(string keyCard)
        {
            ref var enemyViewCardTimeComponent = ref _dataWorld.Select<EnemyTurnViewUIComponent>()
                .SelectFirst<TimeComponent>();

            enemyViewCardTimeComponent.Time = automatic_close_ui_time;
            
            var viewEnemyConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ViewEnemySO;
            var playerEnemyTurnActionUIMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PlayerEnemyTurnActionUIMono;
            var cardMono = playerEnemyTurnActionUIMono.CreateNewCard(viewEnemyConfig.CardForEnemyTurnView);
            
            SetupCardAction.SetViewCardNotInit?.Invoke(cardMono, keyCard);
        }

        private async void HideView()
        {
            var turnUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.CoreHudUIMono.PlayerEnemyTurnActionUIMono;
            turnUI.DisableFrame();
            
            _dataWorld.Select<EnemyTurnViewUIComponent>()
                .SelectFirstEntity()
                .Destroy();
            
            await Task.Delay(wait_time_end_animations);
            turnUI.ClearContainerCard();
        }

        public void Destroy()
        {
            EnemyTurnViewUIAction.ShowViewEnemyCard -= ShowViewCard;
            EnemyTurnViewUIAction.ForceHideView -= HideView;
        }
    }
}