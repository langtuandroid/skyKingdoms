using Managers;
using Service;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("CONFIGURATION\n")]
    [Tooltip("Velocidad del personaje.")]
    [SerializeField]
    private float speed;
    
    [Tooltip("Rotación del personaje.")]
    [SerializeField]
    private float rotationSpeed;
    
    #region REFERENCES
    private Rigidbody rb;
    private Animator animator;
    private Vector3 movement;
    #endregion
    
    private void OnEnable()
    {
        ServiceLocator.GetService<MyInputManager>().movementAction.performed += OnMovementPerformed;
        ServiceLocator.GetService<MyInputManager>().movementAction.canceled += OnMovementCanceled;
    }

    private void OnDisable()
    {
        ServiceLocator.GetService<MyInputManager>().movementAction.performed -= OnMovementPerformed;
        ServiceLocator.GetService<MyInputManager>().movementAction.canceled -= OnMovementCanceled;
    }
    
    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }
    
    private void FixedUpdate()
    {
        FixedControls();   
    }

    private void LateUpdate()
    {
        animator.SetFloat("speed", movement.magnitude);
        animator.SetBool("walk", movement.magnitude > 0f);
    }
    
    #region MOVEMENT
    public void OnMovementPerformed(InputAction.CallbackContext callbackContext)
    {
        movement = callbackContext.ReadValue<Vector2>();
    }

    public void OnMovementCanceled(InputAction.CallbackContext callbackContext)
    {
        movement = Vector2.zero;
    }
    
    void FixedControls()
    {
        // Movimiento del personaje
        Vector3 direction = new Vector3(movement.x, 0, movement.y);

        // Calcula la dirección del movimiento relativa a la cámara
        Vector3 relativeDirection = Camera.main.transform.TransformDirection(direction);
        relativeDirection.y = 0;
        relativeDirection.Normalize();

        // MOVIMIENTO
        Vector3 move = relativeDirection * speed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        // ROTACION
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(relativeDirection, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion
}
