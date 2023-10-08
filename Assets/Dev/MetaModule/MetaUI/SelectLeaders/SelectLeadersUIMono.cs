using System;
using System.Threading.Tasks;
using CyberNet.Core;
using CyberNet.Global;
using DG.Tweening;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Meta
{
    public class SelectLeadersUIMono : MonoBehaviour
    {
        [Header("Global UI")]
        public GameObject Background;
        public GameObject Panel;

        [Header("Select Leaders Info")]
        [SerializeField]
        private RectTransform _frameSelectPlayer;
        public Image SelectLeadersImageCard;
        public Localize SelectLeadersNameLoc;
        public Localize SelectLeadersDescrLoc;
        
        [Header("Select Leaders Ability")]
        public Image SelectLeadersAbilityImage;
        public Localize SelectLeadersAbilityNameLoc;
        public Localize SelectLeadersAbilityDescrLoc;

        [Header("Header 2 current select player")]
        public TextMeshProUGUI HeaderSelectTargetPlayer;

        private Sequence _sequence;
        
        public void Awake()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }

        public void OpenWindow()
        {
            Background.SetActive(true);
            Panel.SetActive(true);
        }

        public void CloseWindow()
        {
            Background.SetActive(false);
            Panel.SetActive(false);
        }
        
        public void SetSelectViewLeader(Sprite imageCard, string name, string descr)
        {
            SelectLeadersImageCard.sprite = imageCard;
            SelectLeadersNameLoc.Term = name;
            SelectLeadersDescrLoc.Term = descr;
        }

        public void SetSelectViewLeaderAbility(Sprite imageAbility, string name, string descr)
        {
            SelectLeadersAbilityImage.sprite = imageAbility;
            SelectLeadersAbilityNameLoc.Term = name;
            SelectLeadersAbilityDescrLoc.Term = descr;
        }

        public void SetLocSelectPlayer(string namePlayer)
        {
            HeaderSelectTargetPlayer.text = namePlayer;
        }

        public void OnClickBackMainMenu()
        {
            SelectLeaderAction.BackMainMenu?.Invoke();
        }

        public void OnClickConfirmSelect()
        {
            SelectLeaderAction.ConfirmSelect?.Invoke();
        }
    }
}