using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Arena
{
    public class ArenaMono : MonoBehaviour
    {
        [SerializeField]
        private List<SlotUnit> _rightUnitPosition = new List<SlotUnit>();
        [SerializeField]
        private List<SlotUnit> _leftUnitPosition = new List<SlotUnit>();

        public void ClearArena()
        {
            foreach (var unitSlot in _rightUnitPosition)
            {
                unitSlot.SlotOccupied = false;
                foreach (Transform unitChild in unitSlot.UnitPosition)
                {
                    Destroy(unitChild.gameObject);
                }
            }
            
            foreach (var unitSlot in _leftUnitPosition)
            {
                unitSlot.SlotOccupied = false;
                foreach (Transform unitChild in unitSlot.UnitPosition)
                {
                    Destroy(unitChild.gameObject);
                }
            }
        }

        public void InitUnitInPosition(UnitArenaMono unitMono, GameObject vfxUnit)
        {
            
        }
    }

    [Serializable]
    public class SlotUnit
    {
        public int IDSlot;
        public Transform UnitPosition;
        public bool SlotOccupied;
    }
}