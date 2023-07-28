using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using System.Collections.Generic;
using ModulesFramework.Systems;
using Newtonsoft.Json;

namespace CyberNet.Core.Dialog
{
    [EcsSystem(typeof(GlobalModule))]
    public class DialogConfigExtract : IInitSystem
    {
        private DataWorld _dataWorld;

        public void Init()
        {
            ref var dialogData = ref _dataWorld.OneData<DialogConfigData>();
            dialogData.DialogConfig = JsonConvert.DeserializeObject<Dictionary<string, DialogConfig>>(dialogData.DialogConfigSO.DialogConfigJson.text);
            dialogData.CharacterDialogConfig = JsonConvert.DeserializeObject<Dictionary<string, CharacterDialogConfig>>(dialogData.DialogConfigSO.CharacterDialogConfigJson.text);
        }
    }
}