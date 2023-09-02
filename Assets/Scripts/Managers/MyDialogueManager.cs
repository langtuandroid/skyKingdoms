using System;
using System.Reflection;
using Doublsb.Dialog;
using Service;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class MyDialogueManager : InputsUI
    {
        [Tooltip("Activar si el player tiene el control de la escena.")]
        public bool PlayerControl;
        public int Step => step;

        [SerializeField] private DialogManager dialogManager;
        [SerializeField] private GameObject nextBtn;
        [SerializeField] private Text characterText;
        [SerializeField] private DialogueOptions dialogueOptions;
        [SerializeField] private TextMeshProUGUI optionA_Text;
        [SerializeField] private TextMeshProUGUI optionB_Text;

        private DOTDialogAnimator dialogAnimator;

        private MethodInfo storyMethod;
        private MethodInfo storyMaxStepMethod;
        private string currentText;
        private int step = 0;
        private int maxStep = 0;
        private string currentScene;
        private bool isSubmitBtn;
        private bool canStart;
        private const string TEXT_STORY = "Text_Story";
        private const string TEXT = "Text";
        private const string MAX_STEPS = "GetMaxStep";

        private void Start()
        {
            dialogAnimator = GetComponent<DOTDialogAnimator>();
        }

        void LateUpdate()
        {
            if (canStart && PlayerControl)
            {
                CheckShowButton();   
            } else if (canStart && !PlayerControl)
            {

            }
        }

        public void Init()
        {
            step = 0;
            currentScene = GetActiveStoryScene();
            Type storyType = Type.GetType(currentScene);
            if (storyType != null)
            {
                storyMethod = storyType.GetMethod(TEXT);
                storyMaxStepMethod = storyType.GetMethod(MAX_STEPS);

                if (storyMaxStepMethod != null)
                {
                    maxStep = (int)storyMaxStepMethod.Invoke(null, null);
                }
                
                
                if (storyMethod != null)
                {
                    dialogAnimator.ShowDialogBox();
                    canStart = true;
                    NewDialogText();
                }
            }
        }
        
        // Recojo la escena actual
        string GetActiveStoryScene()
        {
            string scene = SceneManager.GetActiveScene().name;
            return TEXT_STORY + scene.Substring(scene.IndexOf("_"));
        }

        // Muestro nuevo texto
        private void NewDialogText()
        {
            if (step < maxStep)
            {
                step++;

                currentText = (string)storyMethod.Invoke(null, new object[] { step });

                int asteriskIndex = currentText.IndexOf("*");
                
                string characterName = currentText.Substring(0, asteriskIndex);

                DialogData dialogData = new DialogData(currentText.Substring(asteriskIndex + 1));

                characterText.text = characterName;

                //canCheckVisibility = true;

                dialogAnimator.ShowDialogBox();
                
                dialogManager.Show(dialogData);
            }
            else
            {
                StoryEnds();
            }
        }
        
        // Dialogos con opciones
        public void NewOptionText(string text, string character, string textA, string textB, bool showOptions)
        {
            currentText = text;

            string characterName = character;

            optionA_Text.text = textA;

            optionB_Text.text = textB;

            DialogData dialogData = new DialogData(currentText);

            characterText.text = characterName;

            //canCheckVisibility = true;
            
            dialogAnimator.ShowDialogBox();

            dialogManager.Show(dialogData);
            
            if (showOptions)
                dialogueOptions.ShowOptions();
        }
        
        // Para cuando el Player controla el botón
        void CheckShowButton()
        {
            var isActivated = CanContinue();
            nextBtn.SetActive(isActivated);   
            isSubmitBtn = !isActivated;
        }

        // Para cuando la cinemática controla los cambios de texto
        public bool CanContinue()
        {
            int asteriskIndex = currentText.IndexOf("*");
            string text = currentText.Substring(asteriskIndex + 1);

            return dialogManager.Printer_Text.text.Length >= text.Length;
        }
        
        // Player Input Action Submit
        protected override void OnSubmit(InputAction.CallbackContext context)
        {
            base.OnSubmit(context);
            
            if (PlayerControl)
            {
                if (nextBtn.activeSelf && !isSubmitBtn)
                {
                    isSubmitBtn = true;
                    NewDialogText();   
                }   
            }
        }
        
        // Siguiente texto controlado por la cinemática
        public void NextText()
        {
            if (!PlayerControl)
            {
                dialogAnimator.ShowDialogBox();
                NewDialogText();   
            }
        }
        
        // Carga de textos en Niveles
        public void TextLevel(string level)
        {
            PlayerControl = true;
            
            step = 0;
            
            Type storyType = Type.GetType(TEXT + "_" + level);
 
            if (storyType != null)
            {
                storyMethod = storyType.GetMethod(TEXT);
                storyMaxStepMethod = storyType.GetMethod(MAX_STEPS);

                if (storyMaxStepMethod != null)
                {
                    maxStep = (int)storyMaxStepMethod.Invoke(null, null);
                }
                
                
                if (storyMethod != null)
                {
                    dialogAnimator.ShowDialogBox();
                    canStart = true;
                    NewDialogText();
                }
            }
        }
        
        public void HideDialogBox()
        {
            dialogAnimator.HideDialogBox();
        }

        public void StopStory()
        {
            step = maxStep;
            StoryEnds();
        }
        
        // No hay más texto que mostrar
        void StoryEnds()
        {
            dialogAnimator.HideDialogBox();
            
            ServiceLocator.GetService<MyGameManager>().ResumePlayerMovement();
        }
    }
}
