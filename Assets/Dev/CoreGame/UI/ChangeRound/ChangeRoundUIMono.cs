using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using I2.Loc;
using DG.Tweening;

namespace CyberNet.Core.UI
{
    public class ChangeRoundUIMono : MonoBehaviour
    {
        public GameObject NewRoundGO;
        public Localize TextRound;

        private Sequence _sequence;
        //TODO add animations and set text round
        public async void OnNewRound()
        {
            //TextRound.Term = text;
            NewRoundGO.SetActive(true);
            await Task.Delay(750);
            NewRoundGO.SetActive(false);
        }
    }
}