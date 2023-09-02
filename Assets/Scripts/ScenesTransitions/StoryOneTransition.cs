using System;
using Managers;
using Service;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class StoryOneTransition : MonoBehaviour
{
    public static StoryOneTransition Instance;
    private bool textShowed;
    private bool CanCheck;

    private void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (gameObject.name)
        {
            case "Story_1Transition":
                ServiceLocator.GetService<MyLevelManager>().backToScene = true;
                SceneManager.LoadScene("Story_1");
                break;
            case "DragonTransition":
                if (!other.CompareTag(Constants.Player) || textShowed || !CanCheck) return;
                textShowed = true;
    
                ServiceLocator.GetService<MyDialogueManager>().NewOptionText(Text_Story_1.OptionText, Constants.Dragon, "", "", true);
                break;
        }
    }
    
    
    private void OnTriggerExit(Collider other)
    {
        textShowed = false;
    }

    public void CanCheckDialogueOptions()
    {
        CanCheck = true;
    }
}
