using System;
using Managers;
using Service;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerCustom : InputsUI
    {
        [SerializeField] GameObject Boy;
        [SerializeField] GameObject Girl;
        
        void Start()
        {
            Girl.transform.rotation = Quaternion.Euler(0f, 00f, 0f);
            Boy.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            ServiceLocator.GetService<MyDialogueManager>().NewOptionText(Text_ChooseCharacter.OptionText, "", Text_ChooseCharacter.OptionA, Text_ChooseCharacter.OptionB, true);
        }

        private void Update()
        {
            if (ActualOption == 0)
            {
                Girl.transform.rotation = Quaternion.Euler(0f, 00f, 0f);
                Boy.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else
            {
                Girl.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                Boy.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }


        protected override void OnSubmit(InputAction.CallbackContext context)
        {
            base.OnSubmit(context);
            
            if (ActualOption == 0)
                ServiceLocator.GetService<PlayerData>().PlayerSO.player = "B";
            else 
                ServiceLocator.GetService<PlayerData>().PlayerSO.player = "G";
            
            //ServiceLocator.GetService<LoadScreenManager>().LoadScene("Story_0");
            ServiceLocator.GetService<LoadScreenManager>().LoadScene("Test");
        }
    }
}
