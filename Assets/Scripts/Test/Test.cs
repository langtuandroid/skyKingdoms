using Managers;
using Service;
using UnityEngine;

namespace Test
{
    public class Test : MonoBehaviour
    {
        void Start()
        {
            ServiceLocator.GetService<MyInputManager>().PlayerInputs();
            ServiceLocator.GetService<MyLevelManager>().StartLevel();
        }
    }
}