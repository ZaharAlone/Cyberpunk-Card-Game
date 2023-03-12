using UnityEngine;
using EcsCore;
using ModulesFrameworkUnity;

public class InitBoardGameModule : MonoBehaviour
{
    public void Start()
    {
        ModulesUnityAdapter.world.ActivateModule<BoardGameModule>();
    }
}