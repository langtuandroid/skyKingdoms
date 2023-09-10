using Managers;
using UnityEngine;

namespace Service
{
    [DefaultExecutionOrder(-100)]
    public class ServiceLocatorInitializer : MonoBehaviour
    {
        private void Awake()
        {
            if (!ServiceLocator.IsInitialized)
            {
                AddServices();
                ServiceLocator.IsInitialized = true;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void AddServices()
        {
            var loadScreenManager = FindObjectOfType<LoadScreenManager>();
            ServiceLocator.AddService(loadScreenManager);

            var myGameManager = FindObjectOfType<MyGameManager>();
            ServiceLocator.AddService(myGameManager);

            var myInputManager = FindObjectOfType<MyInputManager>();
            ServiceLocator.AddService(myInputManager);

            var myAudioManager = FindObjectOfType<MyAudioManager>();
            ServiceLocator.AddService(myAudioManager);

            var myLevelManager = FindObjectOfType<MyLevelManager>();
            ServiceLocator.AddService(myLevelManager);

            var myDialogueManager = FindObjectOfType<MyDialogueManager>();
            ServiceLocator.AddService(myDialogueManager);
            
            var myPlayerData = FindObjectOfType<PlayerData>();
            ServiceLocator.AddService(myPlayerData);

            var dayNightSystem = FindObjectOfType<DayNightSystem>();
            ServiceLocator.AddService(dayNightSystem);
            
            var cameraController = FindObjectOfType<CameraController>();
            ServiceLocator.AddService(cameraController);
            
            var impacts = FindObjectOfType<Impacts>();
            ServiceLocator.AddService(impacts);
        }
    }
}