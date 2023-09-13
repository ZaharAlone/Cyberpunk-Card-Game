using UnityEngine;
namespace CyberNet.Core.City
{
    public class SolidPointVFXMono : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer LightCube;
        [SerializeField]
        private ParticleSystem GroundParticle;

        public void SetColor(Color32 color)
        {
            var material = LightCube.materials[0];
            material.SetColor("_TintColor", new Color32(color.r, color.g, color.b, 18));
            LightCube.materials[0] = material;
            GroundParticle.startColor = new Color32(color.r, color.g, color.b, 134);
        }
    }
}