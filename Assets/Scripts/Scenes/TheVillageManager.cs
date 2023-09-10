using Managers;
using Service;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class TheVillageManager : InputsUI
{
    public GameObject TheVillageUI;
    
    protected override void OnSubmit(InputAction.CallbackContext context)
    {
        base.OnSubmit(context);
        
        TheVillageUI.SetActive(false);
        
        ServiceLocator.GetService<MyLevelManager>().StartLevel();
    }
}
