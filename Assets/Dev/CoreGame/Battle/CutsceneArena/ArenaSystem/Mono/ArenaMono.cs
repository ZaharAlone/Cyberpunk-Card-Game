using System;
using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.Battle.CutsceneArena
{
    public class ArenaMono : MonoBehaviour
    {
        [SerializeField]
        private GameObject _arenaContainer;
        [SerializeField]
        private GameObject _camera;
        public ArenaCameraMono ArenaCameraMono;
        
        [SerializeField]
        private List<SlotUnit> _rightUnitPosition = new List<SlotUnit>();
        [SerializeField]
        private List<SlotUnit> _leftUnitPosition = new List<SlotUnit>();
        
        public void OnEnable()
        {
            DisableArena();
            DisableCamera();
        }

        public void EnableCamera()
        {
            _camera.SetActive(true);
        }
        
        public void DisableCamera()
        {
            _camera.SetActive(false);
        }

        public void EnableArena()
        {
            _arenaContainer.SetActive(true);
        }

        public void DisableArena()
        {
            _arenaContainer.SetActive(false);
            ClearArena();
        }
        
        private void ClearArena()
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
        
        public void InitUnitInPosition(UnitArenaMono unitMono, bool isLeft) //, GameObject vfxUnit
        {
            if (isLeft)
            {
                SetUnitPositionOnArena(unitMono, _leftUnitPosition);
            }
            else
            {
                SetUnitPositionOnArena(unitMono, _rightUnitPosition);
            }
        }

        private void SetUnitPositionOnArena(UnitArenaMono unitMono, List<SlotUnit> slots)
        {
            foreach (var slot in slots)
            {
                if (!slot.SlotOccupied)
                {
                    slot.SlotOccupied = true;
                    unitMono.transform.SetParent(slot.UnitPosition);
                    unitMono.transform.localPosition = Vector3.zero;
                    unitMono.transform.localRotation = Quaternion.identity;
                    
                    break;
                }
            }
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