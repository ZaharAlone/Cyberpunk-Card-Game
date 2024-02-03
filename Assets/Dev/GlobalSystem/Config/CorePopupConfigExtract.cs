using EcsCore;
using System.Collections.Generic;
using Newtonsoft.Json;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.UI.CorePopup
{
    /// <summary>
    /// Читаем Json с конфигом попапа и записываем его в компонент
    /// </summary>
    [EcsSystem(typeof(GlobalModule))]
    public class CorePopupConfigExtract : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            var boardGameData = _dataWorld.OneData<BoardGameData>();
            var popupConfig = JsonConvert.DeserializeObject<Dictionary<string, CorePopupConfig>>(boardGameData.BoardGameConfig.PopupCardConfigJson.text);
            var popupTaskConfig = JsonConvert.DeserializeObject<Dictionary<string, CorePopupTaskConfig>>(boardGameData.BoardGameConfig.PopupTaskConfigJson.text);
            
            _dataWorld.CreateOneData(new CorePopupData 
            {
                CorePopupConfig = popupConfig,
                CorePopupTaskConfig = popupTaskConfig
            });
        }
    }
}