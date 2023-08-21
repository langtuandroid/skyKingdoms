using System;
using System.Collections;
using AI.Player_Controller;
using Player;
using Service;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MyGameManager : MonoBehaviour
    {
        #region VARIABLES
        public GameObject GameOverCanvas;
        public GameObject animationGameOver;
        public GameObject menuGameOver;
        public Animator playerCanvasAnimator;
        public bool gameOver;
        public bool isLoading;
        public bool AIDemoControl;
        
        [SerializeField]
        private bool _playerControl;
        public event Action OnPlayerControl;

        private string _character = "M";

        public string Character
        {
            get => _character;
            set => _character = value;
        }
        
        #endregion
    
        #region UNITY METHODS

        private void Start()
        {
           if(_playerControl)
                OnPlayerControl?.Invoke();
        }
        
        private void OnEnable()
        {
            OnPlayerControl += ResumePlayerMovement;
        }

        private void OnDisable()
        {
            OnPlayerControl -= ResumePlayerMovement;
        }

        #endregion
    
        #region INIT CONFIGURATION
        public void Init()
        {
            var sceneName = SceneManager.GetActiveScene().name;

            switch (sceneName)
            {
                case "Cinematic":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("Cinematic");
                    break;
                case "Level1":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("dayAmbient");
                    ServiceLocator.GetService<MyLevelManager>().Level("level1");
               
                    GameObject Player = GameObject.FindWithTag(Constants.PLAYER);
                    if (Player == null) return;
        
                    if (AIDemoControl)
                    {
                        Player.GetComponent<BoyController>().enabled = false;
                        Player.GetComponent<AIController>().enabled = true;
                    }
                    break;
                case "Level2":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("dungeon");
                    ServiceLocator.GetService<MyLevelManager>().Level("level2", true);
                    break;
                case "Flight":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("flight");
                    break;
                case "BossBattle":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("boss");
                    ServiceLocator.GetService<MyLevelManager>().Level("level3", true);
                    break;
                case "TheEnd":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("theEnd");
                    break;
                case "Story_0":
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("dungeon");
                    ServiceLocator.GetService<MyLevelManager>().Level("Story_0", true);
                    break;
                case "Story_1":
                    if (ServiceLocator.GetService<MyLevelManager>().backToScene) return;
                    ServiceLocator.GetService<MyAudioManager>().PlayMusic("town");
                    break;
                default:
                    Debug.Log("No music assigned for scene: " + sceneName);
                    ServiceLocator.GetService<MyLevelManager>().Level(sceneName);
                    break;
            }
        }

        #endregion
    
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
    
        #region GAME OVER
        public void GameOver()
        {
            if (isLoading) return; //TODO meter pequeño delay
        
            if (!gameOver)
            {
                gameOver = true;
               // MyAudioManager.Instance.StopAny();
                //MyAudioManager.Instance.PlaySfx("gameOverSFX");
                GameOverCanvas.SetActive(true);
                StartCoroutine(nameof(GameOverAnimation));
            }
        }

        IEnumerator GameOverAnimation()
        {
            animationGameOver.SetActive(true);
        
            playerCanvasAnimator.SetTrigger("dead");

            yield return new WaitForSeconds(3f);

            menuGameOver.SetActive(true);
        }

        public void RestartLevel()
        {
            playerCanvasAnimator.SetTrigger("continue");
        }
        #endregion

        #region PAUSE GAME
        public void PausePlayerMovement()
        {
        
        }

        public static void ResumePlayerMovement()
        {
          
        }
        #endregion
    }
}
