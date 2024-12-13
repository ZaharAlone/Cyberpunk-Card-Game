using CyberNet.Global.Sound;
using FMODUnity;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Meta.SelectPlayersForGame
{
    public class SelectPlayerSlotUIMono : MonoBehaviour
    {
        public int IDPlayerSlot;
        public bool AlwaysPlayer;
        public bool NotClear;

        [Header("Slot state")]
        public GameObject LeaderSlotGO;
        public GameObject SelectSlotGO;
        public GameObject ClearSlotGO;
        public SelectImageUIMono SelectImageButtonClearSlot;

        [Header("Leader View")]
        public Image LeaderImage;
        public Image FrameLeaderImage;
        public Image LeaderAbility;
        public Image AbilityFrame;
        public Image LineDownBar;
        public Localize LeaderAbilityName;
        public TextMeshProUGUI LeaderAbilityText;

        public Color32 FrameBaseColor;
        public Color32 FrameDeleteColor;

        [Header("Player name")]
        public GameObject NamePlayerContainerGO;
        public TextMeshProUGUI NamePlayer;
        public GameObject NamePlayerInput;
        public TextMeshProUGUI DefaultPlayerNameInput;
        public TMP_InputField InputField;

        [SerializeField]
        private GameObject _iconsEditName;
        [SerializeField]
        private GameObject _iconsApplyNewName;
        
        [Header("Low bar")]
        public GameObject SwitchEnemyGO;
        public TextMeshProUGUI CurrentEnemyText;

        public void OnEnable()
        {
            SwitchEnemyGO.SetActive(!AlwaysPlayer);
            SelectImageButtonClearSlot.enabled = !NotClear;
        }

        public void SetBaseNamePlayer(string namePlayer)
        {
            NamePlayerContainerGO.SetActive(true);
            NamePlayerInput.SetActive(false);
            NamePlayer.text = namePlayer;
        }

        public void SetCustomNamePlayer(string namePlayer)
        {
            NamePlayerContainerGO.SetActive(false);
            NamePlayerInput.SetActive(true);
            DefaultPlayerNameInput.text = namePlayer;
            InputField.text = namePlayer;
        }
        
        public void SetViewLeader(Sprite leaderSprite)
        {
            LeaderImage.sprite = leaderSprite;
            RecolorUIElement(FrameBaseColor);
        }

        public void SetLocTypePlayer(string nameTypePlayer)
        {
            CurrentEnemyText.text = nameTypePlayer;
        }

        public void OnClickSwitchEnemyLeft()
        {
            SelectPlayerAction.SwitchTypePlayer?.Invoke(IDPlayerSlot, false);
        }
        
        public void OnClickSwitchEnemyRight()
        {
            SelectPlayerAction.SwitchTypePlayer?.Invoke(IDPlayerSlot, true);
        }

        public void OnClickEditLeader()
        {
            SelectPlayerAction.OnClickEditLeader?.Invoke(IDPlayerSlot);
        }

        public void OnClickOpenSlot()
        {
            OpenSlot();
            SelectPlayerAction.CreatePlayer?.Invoke(IDPlayerSlot);
        }

        public void OpenSlot()
        {
            LeaderSlotGO.SetActive(true);
            ClearSlotGO.SetActive(false);
        }
        
        public void OnClickClearSlot()
        {
            ClearSlot();
            SelectPlayerAction.ClearSlot?.Invoke(IDPlayerSlot);
        }

        public void ClearSlot()
        {
            LeaderSlotGO.SetActive(false);
            ClearSlotGO.SetActive(true);
        }

        public void OnSelectSlot()
        {
            SelectSlotGO.SetActive(true);
            RecolorUIElement(FrameDeleteColor);
        }

        public void OffSelectSlot()
        {
            SelectSlotGO.SetActive(false);
            RecolorUIElement(FrameBaseColor);
        }

        private void RecolorUIElement(Color32 color)
        {
            FrameLeaderImage.color = color;
            LeaderAbility.color = color;
            AbilityFrame.color = color;
            LineDownBar.color = color;
            LeaderAbilityText.color = color;
        }

        public void OnClickEditName()
        {
            _iconsApplyNewName.SetActive(true);
            _iconsEditName.SetActive(false);
        }

        public void OnDeselectEditName()
        {
            OnClickEndEditPlayerName();
        }
        
        public void OnClickEndEditPlayerName()
        {
            var newName = InputField.text;
            SelectPlayerAction.SetPlayerName?.Invoke(IDPlayerSlot, newName);
            
            _iconsApplyNewName.SetActive(false);
            _iconsEditName.SetActive(true);
        }
    }
}