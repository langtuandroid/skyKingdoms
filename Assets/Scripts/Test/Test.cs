using Managers;
using Service;
using UnityEngine;

namespace Test
{
    public class Test : MonoBehaviour
    {
        void Start()
        {
            ServiceLocator.GetService<PlayerData>().PlayerInstantation();
            ServiceLocator.GetService<MyGameManager>().ResumePlayerMovement();
        }
    }
}