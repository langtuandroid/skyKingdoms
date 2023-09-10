using System;
using System.Collections;
using Service;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MyLevelManager : MonoBehaviour
    {
        public bool AIDemoControl;
        public int enemyCount { get; set; }
        public bool backToScene;

        public GameObject Gem_Level1;
        public GameObject Gem_Level_Boss;
        public GameObject Goblin;
        public GameObject GoblinWeapon;

        public event Action OnLevelInit;
        
        public void StartLevel()
        {
            var sceneName = SceneManager.GetActiveScene().name;
            
            switch (sceneName)
            {
                case "Cinematic":
                    InitializeCinematic();
                    break;
                case "TheVillage":
                    InitializeTheVillage();
                    break;
                case "Story_0":
                    InitializeStory0();
                    break;
                case "Story_1":
                    InitializeStory1();
                    break;
                case "Level1":
                    InitializeLevel1();
                    break;
                case "Level2":
                    InitializeLevel2();
                    break;
                case "level3":
                    InitializeLevel3();
                    break;
                case "Flight":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("flight");
                    break;
                case "BossBattle":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("boss");
                    break;
                case "TheEnd":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("theEnd");
                    break;
            }
        }
        
        #region Cinematics
        private void InitializeCinematic()
        {
            //ServiceLocator.GetService<PlayerController>().IdleNoWeapon = true;
            ServiceLocator.GetService<MyAudioManager>().PlayMusic("Cinematic");   
        }
        
        private void InitializeStory0()
        {
            //ServiceLocator.GetService<MyDialogueManager>().PlayerControl = false;
            ServiceLocator.GetService<MyAudioManager>().PlayMusic("dungeon");
        }
        
        private void InitializeStory1()
        {
            if (backToScene)
            {
                backToScene = false;
                var backPosition = GameObject.FindWithTag("BackPosition")?.GetComponent<Transform>().position;
                //BoyController.Instance.SetPosition(backPosition);
            }

            if (ServiceLocator.GetService<MyLevelManager>().backToScene) return;
            
            ServiceLocator.GetService<PlayerData>().PlayerInstantation();
            ServiceLocator.GetService<MyAudioManager>().PlayMusic("town");
            ServiceLocator.GetService<MyDialogueManager>().PlayerControl = false;
        }
        #endregion
        
        private void InitializeTheVillage()
        {
            ServiceLocator.GetService<PlayerData>().PlayerInstantation();
            ServiceLocator.GetService<MyGameManager>().ResumePlayerMovement();
        }
        
        
        #region Levels
        private void InitializeLevel1()
        {
            GameObject player = new GameObject();
            player.transform.position = new Vector3(8f, 1f, 8f);
            ServiceLocator.GetService<PlayerData>().PlayerInstantation(player.transform);
            Destroy(player);
            
            ServiceLocator.GetService<MyAudioManager>().PlayMusic("dayAmbient");
            ServiceLocator.GetService<MyDialogueManager>().TextLevel("Level1");
            OnLevelInit?.Invoke();
        }

        private void InitializeLevel2()
        {
            ServiceLocator.GetService<PlayerData>().PlayerInstantation();
            ServiceLocator.GetService<MyAudioManager>().PlayMusic("dungeon");
            ServiceLocator.GetService<MyDialogueManager>().TextLevel("Level2");
        }

        private void InitializeLevel3()
        {
            ServiceLocator.GetService<MyDialogueManager>().TextLevel("Level3");
        }
        
        
        #endregion

        //TODO sacar de este script
        private void CheckEnemyCounter()
        {
            if (enemyCount >= 5)
            {
                Gem_Level1.SetActive(true);
            }
        }
        
        private IEnumerator StartGoblinIA()
        {
            yield return new WaitForSeconds(2f);
            Goblin.SetActive(true);
            GoblinWeapon.SetActive(true);
        }
        
        #region COLLECTIBLES
        public void CollectGem(Collectible.CollectibleTypes gemType)
        {
            switch (gemType)
            {
                case Collectible.CollectibleTypes.GemBlue:
                    Debug.Log("Azul conseguida");
                    break;
                case Collectible.CollectibleTypes.GemPurple:
                    FlightLevel.Instance.LevelComplete();
                    break;
                case Collectible.CollectibleTypes.GemRed:
                    Debug.Log("Roja conseguida");
                    break;
                case Collectible.CollectibleTypes.GemGreen:
                    Debug.Log("Verde conseguida");
                    break;
            }
        }
        #endregion
    }
}
