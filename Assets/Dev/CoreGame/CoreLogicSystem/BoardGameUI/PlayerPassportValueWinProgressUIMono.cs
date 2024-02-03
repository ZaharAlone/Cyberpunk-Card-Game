using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CyberNet.Core.UI
{
    public class PlayerPassportValueWinProgressUIMono : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> _valueBar = new List<GameObject>();

        public void SetCountValue(int value)
        {
            for (int i = 0; i < _valueBar.Count; i++)
            {
                _valueBar[i].SetActive(i < value);
            }
        }

        [Button("Get element")]
        public void GetElementList()
        {
            _valueBar.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                _valueBar.Add(transform.GetChild(i).gameObject);
            }
        }
    }
}