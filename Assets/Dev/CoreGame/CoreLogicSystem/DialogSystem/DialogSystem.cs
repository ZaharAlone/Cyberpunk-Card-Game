using CyberNet.Core.UI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Core.Dialog
{
    [EcsSystem(typeof(CoreModule))]
    public class DialogSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            DialogAction.StartDialog += StartDialog;
            DialogAction.NextDialog += NextDialog;
        }

        private void StartDialog(string keyDialog)
        {
            _dataWorld.CreateOneData(new CurrentDialogData { DialogKey = keyDialog, CurrentIndexPhrase = 0 });
            ShowDialog(0);
        }

        private void ShowDialog(int phrase)
        {
            ref var dialogData = ref _dataWorld.OneData<CurrentDialogData>();
            ref var dialogUI = ref _dataWorld.OneData<UIData>().UIMono.DialogUIMono;
            ref var dialogConfigData = ref _dataWorld.OneData<DialogConfigData>();

            dialogData.CurrentIndexPhrase = phrase;
            dialogConfigData.DialogConfig.TryGetValue(dialogData.DialogKey, out var currentDialog);
            var currentPhrase = currentDialog.Phrase[phrase];
            dialogConfigData.CharacterDialogConfig.TryGetValue(currentPhrase.Character, out var characterView);
            dialogConfigData.DialogConfigSO.DialogAvatars.TryGetValue(characterView.Image_key, out var characterImage);

            dialogUI.SetViewDialog(characterImage, characterView.Loc_name, currentPhrase.Dialog);

            if (phrase == 0)
                dialogUI.OpenDialog();
        }

        private void EndDialog()
        {
            ref var dialogUI = ref _dataWorld.OneData<UIData>().UIMono.DialogUIMono;
            dialogUI.CloseDialog();
            ref var dialogData = ref _dataWorld.OneData<CurrentDialogData>();
            ref var dialogConfigData = ref _dataWorld.OneData<DialogConfigData>();
            dialogConfigData.DialogConfig.TryGetValue(dialogData.DialogKey, out var currentDialog);
            
            _dataWorld.RemoveOneData<CurrentDialogData>();
            DialogAction.EndDialog?.Invoke();
        }

        private void NextDialog()
        {
            ref var CurrentDialogData = ref _dataWorld.OneData<CurrentDialogData>();
            ref var dialogConfigData = ref _dataWorld.OneData<DialogConfigData>();
            dialogConfigData.DialogConfig.TryGetValue(CurrentDialogData.DialogKey, out var currentDialog);
            if (CurrentDialogData.CurrentIndexPhrase + 1 < currentDialog.Phrase.Count)
                ShowDialog(CurrentDialogData.CurrentIndexPhrase + 1);
            else
                EndDialog();
        }
    }
}