using System;
using UnityEngine;
using UnityEngine.Serialization;
namespace CyberNet.Core.City
{
    public class UnitPointVFXMono : MonoBehaviour
    {
        [FormerlySerializedAs("LightCube")]
        [SerializeField]
        private MeshRenderer _lightCube;
        [FormerlySerializedAs("GroundParticle")]
        [SerializeField]
        private ParticleSystem _groundParticle;

        public void OnEnable()
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
        }
        
        public void SetColor(Color32 color)
        {
            var material = _lightCube.materials[0];
            material.SetColor("_TintColor", new Color32(color.r, color.g, color.b, 18));
            _lightCube.materials[0] = material;
            _groundParticle.startColor = new Color32(color.r, color.g, color.b, 134);
        }
    }
}