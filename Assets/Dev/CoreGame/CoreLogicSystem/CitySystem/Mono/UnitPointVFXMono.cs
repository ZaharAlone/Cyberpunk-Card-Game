using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace CyberNet.Core.Map
{
    public class UnitPointVFXMono : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer _lightCube;
        [SerializeField]
        private ParticleSystem _groundParticle;
        [SerializeField]
        private GameObject _aimIcons;

        private void OnEnable()
        {
            DisableEffect();
        }

        public void EnableEffect()
        {
            _lightCube.gameObject.SetActive(true);   
            _groundParticle.gameObject.SetActive(true);   
        }

        public void DisableEffect()
        {
            _lightCube.gameObject.SetActive(false);   
            _groundParticle.gameObject.SetActive(false);
            _aimIcons.SetActive(false);
        }
        
        public void SetColor(Color32 color, bool isAttack)
        {
            var material = _lightCube.materials[0];
            material.SetColor("_TintColor", new Color32(color.r, color.g, color.b, 18));
            _lightCube.materials[0] = material;
            _groundParticle.startColor = new Color32(color.r, color.g, color.b, 134);
            _aimIcons.SetActive(isAttack);
        }
    }
}