using System;
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
        public Image LeaderAbility;
        public Localize LeaderName;

        [Header("Player name")]
        public TextMeshProUGUI NamePlayer;
        public GameObject NamePlayerInput;
        public TextMeshProUGUI DefaultPlayerNameInput;

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
            NamePlayer.gameObject.SetActive(true);
            NamePlayerInput.SetActive(false);
            NamePlayer.text = namePlayer;
        }

        public void SetCustomNamePlayer(string namePlayer)
        {
            NamePlayer.gameObject.SetActive(true);
            NamePlayerInput.SetActive(false);
            DefaultPlayerNameInput.text = namePlayer;
        }
        
        public void SetViewLeader(Sprite leaderSprite, Sprite abilitySprite, string leaderName)
        {
            LeaderImage.sprite = leaderSprite;
            LeaderAbility.sprite = abilitySprite;
            LeaderName.Term = leaderName;
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
            LeaderSlotGO.SetActive(true);
            ClearSlotGO.SetActive(false);
            SelectPlayerAction.CreatePlayer?.Invoke(IDPlayerSlot);
        }

        public void OnClickClearSlot()
        {
            LeaderSlotGO.SetActive(false);
            ClearSlotGO.SetActive(true);
            SelectPlayerAction.ClearSlot?.Invoke(IDPlayerSlot);
        }

        public void OnSelectSlot()
        {
            SelectSlotGO.SetActive(true);
        }

        public void OffSelectSlot()
        {
            SelectSlotGO.SetActive(false);
        }
        
        public void OnEndEditPlayerName(string newName)
        {
            SelectPlayerAction.SetPlayerName?.Invoke(IDPlayerSlot, newName);
        }
    }
}