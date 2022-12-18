using UnityEngine;
using EcsCore;
using ModulesFrameworkUnity;

public class InitBoardGameModule : MonoBehaviour
{
    public void Start()
    {
        EcsWorldContainer.World.ActivateModule<BoardGameModule>();
    }
}