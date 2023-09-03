using System;
using System.Collections;
using Service;
using UnityEngine;

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
        #endregion
    
        #region UNITY METHODS

        private void Start()
        {
            PausePlayerMovement();
        }
        
        #endregion
        
        #region GAME OVER
        public void GameOver()
        {
            if (isLoading) return;
        
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
            ServiceLocator.GetService<MyInputManager>().UIInputs();
        }

        public void ResumePlayerMovement()
        {
          ServiceLocator.GetService<MyInputManager>().PlayerInputs();
        }
        #endregion
    }
}
