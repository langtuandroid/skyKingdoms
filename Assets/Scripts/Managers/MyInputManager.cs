using UnityEngine;
using UnityEngine.InputSystem;

namespace Managers
{
    public class MyInputManager : MonoBehaviour
    {
        public InputActionAsset playerInput;
        public InputAction movementAction;
        public InputAction shootAction;
        public InputAction jumpAction;
        public InputAction defenseAction;
        public InputAction specialAttackAction;
        public InputAction attackAction;

        public InputAction uiMovementAction;
        public InputAction submitAction;
        public InputAction anyAction;

        public bool AnyBtnPressed;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (playerInput == null)
            {
                Debug.LogError("Player Input Asset is not assigned in MyInputManager.");
                return;
            }

            // PLAYER
            InputActionMap playerActionMap = playerInput.FindActionMap("Player");
            if (playerActionMap != null)
            {
                movementAction = playerActionMap.FindAction("Movement");
                shootAction = playerActionMap.FindAction("Shoot");
                jumpAction = playerActionMap.FindAction("Jump");
                defenseAction = playerActionMap.FindAction("Defense");
                specialAttackAction = playerActionMap.FindAction("Special Attack");
                attackAction = playerActionMap.FindAction("Attack");
            }
            else
            {
                Debug.LogError("Player action map not found in Player Input Asset.");
            }

            // UI
            InputActionMap uiActionMap = playerInput.FindActionMap("Player UI");
            if (uiActionMap != null)
            {
                uiMovementAction = uiActionMap.FindAction("Direction");
                submitAction = uiActionMap.FindAction("Submit");
                anyAction = uiActionMap.FindAction("Any");
            }
            else
            {
                Debug.LogError("UI action map not found in Player Input Asset.");
            }
        }

        #region ENABLE / DISABLE
        public void PlayerInputs()
        {
            if (uiMovementAction != null)
                uiMovementAction.Disable();

            if (submitAction != null)
                submitAction.Disable();

            if (anyAction != null)
                anyAction.Disable();

            if (movementAction != null)
                movementAction.Enable();

            if (shootAction != null)
                shootAction.Enable();

            if (jumpAction != null)
                jumpAction.Enable();

            if (defenseAction != null)
                defenseAction.Enable();

            if (specialAttackAction != null)
                specialAttackAction.Enable();            
            
            if (attackAction != null)
                attackAction.Enable();
        }

        public void UIInputs()
        {
            if (movementAction != null)
                movementAction.Disable();

            if (shootAction != null)
                shootAction.Disable();

            if (jumpAction != null)
                jumpAction.Disable();

            if (defenseAction != null)
                defenseAction.Disable();

            if (specialAttackAction != null)
                specialAttackAction.Disable();
            
            if (attackAction != null)
                attackAction.Enable();

            if (uiMovementAction != null)
                uiMovementAction.Enable();

            if (submitAction != null)
                submitAction.Enable();

            if (anyAction != null)
                anyAction.Enable();
        }
        #endregion
    }
}
