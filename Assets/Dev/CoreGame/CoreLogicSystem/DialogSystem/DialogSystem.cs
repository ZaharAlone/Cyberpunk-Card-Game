using System.Collections.Generic;
using CyberNet.Core.UI;
using CyberNet.Global.Sound;
using EcsCore;
using I2.Loc;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.Dialog
{
    [EcsSystem(typeof(CoreModule))]
    public class DialogSystem : IPreInitSystem, IRunSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private List<char> _punctuationChar = new List<char> {'.', '!', '?'};
        
        public void PreInit()
        {
            DialogAction.StartDialog += StartDialog;
            DialogAction.ClickContinueButton += ClickContinueButton;
        }

        private void StartDialog(string keyDialog)
        {
            _dataWorld.CreateOneData(new CurrentDialogData { DialogKey = keyDialog, CurrentIndexPhrase = 0 });
            ShowDialog(0);
            BlockCamera();
        }

        private void ShowDialog(int phrase)
        {
            ref var dialogData = ref _dataWorld.OneData<CurrentDialogData>();
            ref var dialogUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DialogUIMono;
            ref var dialogConfigData = ref _dataWorld.OneData<DialogConfigData>();
            
            dialogConfigData.DialogConfig.TryGetValue(dialogData.DialogKey, out var currentDialog);
            var currentPhrase = currentDialog.Phrase[phrase];
            dialogConfigData.CharacterDialogConfig.TryGetValue(currentPhrase.Character, out var characterView);
            dialogConfigData.DialogConfigSO.DialogAvatars.TryGetValue(characterView.Image_key, out var characterImage);
            
            dialogData.CurrentIndexPhrase = phrase;
            var animationsPhraseEntity = _dataWorld.NewEntity();
            var animationsPhraseComponent = new DialogPhraseAnimationsComponent();
            animationsPhraseComponent.CurrentIndexCharacter = 0;
            animationsPhraseComponent.Timer = 0.01f;
            animationsPhraseComponent.CurrentPhraseText = LocalizationManager.GetTranslation(currentPhrase.Dialog);
            animationsPhraseComponent.MaxCharactersInPhrase = animationsPhraseComponent.CurrentPhraseText.Length - 1;
            animationsPhraseEntity.AddComponent(animationsPhraseComponent);
            
            dialogUI.SetViewDialog(characterImage, characterView.Loc_name);
            dialogUI.SetEnableTextToContinue(false);
            
            dialogUI.SetDialogText(animationsPhraseComponent.CurrentPhraseText);
            if (phrase == 0)
                dialogUI.OpenDialog();
        }

        public void Run()
        {
            var isAnimationsPhrase = _dataWorld.Select<DialogPhraseAnimationsComponent>().TrySelectFirstEntity(out var dialogPhraseAnimationsEntity);
            if (isAnimationsPhrase)
            {
                ref var dialogPhraseAnimationsComponent = ref dialogPhraseAnimationsEntity.GetComponent<DialogPhraseAnimationsComponent>();
                if (dialogPhraseAnimationsComponent.Timer <= 0)
                {
                    DialogAnimations();
                }
                else
                {
                    dialogPhraseAnimationsComponent.Timer -= Time.deltaTime;
                }
            }
        }

        private void DialogAnimations()
        {
            var phraseAnimationsEntity = _dataWorld.Select<DialogPhraseAnimationsComponent>().SelectFirstEntity();
            ref var phraseAnimationsComponent = ref phraseAnimationsEntity.GetComponent<DialogPhraseAnimationsComponent>();

            ref var dialogUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DialogUIMono;
            phraseAnimationsComponent.Timer = 0.01f;
            phraseAnimationsComponent.CurrentIndexCharacter += 1;
            dialogUI.SetVisibleCountCharacters(phraseAnimationsComponent.CurrentIndexCharacter);
            
            var newChar = phraseAnimationsComponent.CurrentPhraseText[phraseAnimationsComponent.CurrentIndexCharacter];
            
            foreach (var symbol in _punctuationChar)
            {
                if (newChar == symbol)
                {
                    phraseAnimationsComponent.Timer = 0.015f;
                    break;
                }
            }
            
            if (newChar == ',')
            {
                phraseAnimationsComponent.Timer = 0.012f;
            }
            
            if (phraseAnimationsComponent.CurrentIndexCharacter == phraseAnimationsComponent.MaxCharactersInPhrase)
            {
                EndAnimationsPhrase(false);
            }
        }

        private void EndAnimationsPhrase(bool isForce)
        {
            var phraseAnimationsEntity = _dataWorld.Select<DialogPhraseAnimationsComponent>().SelectFirstEntity();
            ref var dialogUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DialogUIMono;

            if (isForce)
            {
                ref var phraseAnimationsComponent = ref phraseAnimationsEntity.GetComponent<DialogPhraseAnimationsComponent>();
                dialogUI.SetVisibleCountCharacters(phraseAnimationsComponent.MaxCharactersInPhrase);
            }
            
            phraseAnimationsEntity.Destroy();
            dialogUI.SetEnableTextToContinue(true);
        }

        private void EndDialog()
        {
            ref var dialogUI = ref _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.DialogUIMono;
            dialogUI.CloseDialog();
            ref var dialogData = ref _dataWorld.OneData<CurrentDialogData>();
            ref var dialogConfigData = ref _dataWorld.OneData<DialogConfigData>();
            dialogConfigData.DialogConfig.TryGetValue(dialogData.DialogKey, out var currentDialog);
            
            _dataWorld.RemoveOneData<CurrentDialogData>();
            DialogAction.EndDialog?.Invoke();
            UnblockCamera();
        }

        private void ClickContinueButton()
        {
            var isAnimationsPhrase = _dataWorld.Select<DialogPhraseAnimationsComponent>().TrySelectFirstEntity(out var dialogPhraseAnimationsEntity);

            if (isAnimationsPhrase)
            {
                EndAnimationsPhrase(true);
            }
            else
            {
                NextDialog();
            }
            
            SoundAction.PlaySound?.Invoke(_dataWorld.OneData<SoundData>().Sound.DialogNextPhrase);
        }
        
        private void NextDialog()
        {
            ref var CurrentDialogData = ref _dataWorld.OneData<CurrentDialogData>();
            ref var dialogConfigData = ref _dataWorld.OneData<DialogConfigData>();
            dialogConfigData.DialogConfig.TryGetValue(CurrentDialogData.DialogKey, out var currentDialog);
            
            if (CurrentDialogData.CurrentIndexPhrase + 1 < currentDialog.Phrase.Count)
                ShowDialog(CurrentDialogData.CurrentIndexPhrase + 1);
            else
            {
                EndDialog();
            }
        }

        private void BlockCamera()
        {
            var blockCamera = _dataWorld.NewEntity();
            blockCamera.AddComponent(new BlockCameraInputComponent());
        }

        private void UnblockCamera()
        {
            var blockCameraEntity = _dataWorld.Select<BlockCameraInputComponent>().SelectFirstEntity();
            blockCameraEntity.Destroy();
        }

        public void Destroy()
        {
            DialogAction.StartDialog -= StartDialog;
            DialogAction.ClickContinueButton -= NextDialog;
        }
    }
}