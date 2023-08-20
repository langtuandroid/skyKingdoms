using System;
using System.Collections;
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

    [Tooltip("Fuerza del salto.")]
    [SerializeField]
    private float jumpHeight;
    
    [Tooltip("Máscara de capa utilizada para verificar si el personaje está en el suelo.")]
    [SerializeField]
    private LayerMask groundLayerMask;

    #region REFERENCES
    private Rigidbody rb;
    private Animator animator;
    private Vector3 movement;
    #endregion

    #region JUMP VARIABLES
    public float groundCheckDistance = 0.5f;
    private bool isJumping;
    private bool isGrounded = true;
    #endregion

    public bool CanMove = false;

    private void OnEnable()
    {
        ServiceLocator.GetService<MyInputManager>().movementAction.performed += OnMovementPerformed;
        ServiceLocator.GetService<MyInputManager>().movementAction.canceled += OnMovementCanceled;
        ServiceLocator.GetService<MyInputManager>().attackAction.performed += OnAttackPerformed;
        ServiceLocator.GetService<MyInputManager>().jumpAction.performed += OnJumpPerformed;
    }
    
    private void OnDisable()
    {
        ServiceLocator.GetService<MyInputManager>().movementAction.performed -= OnMovementPerformed;
        ServiceLocator.GetService<MyInputManager>().movementAction.canceled -= OnMovementCanceled;
        ServiceLocator.GetService<MyInputManager>().attackAction.performed -= OnAttackPerformed;
        ServiceLocator.GetService<MyInputManager>().jumpAction.performed -= OnJumpPerformed;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        CanMove = true;
    }

    private void Update()
    {
        if (!CanMove) return;
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;
        FixedControls();
    }

    private void LateUpdate()
    {
        animator.SetFloat("speed", movement.magnitude);
        animator.SetBool("walk", (movement.magnitude > 0f));
        animator.SetBool("jump", isJumping);
        animator.SetBool("land", isGrounded);
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

    #region ATTACK
    private void OnAttackPerformed(InputAction.CallbackContext obj)
    {
        animator.SetTrigger("attack");
    }
    #endregion

    #region JUMP
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (!isJumping && isGrounded)
        {
            isJumping = true;
            isGrounded = false;

            animator.Play("JumpStart_Normal_InPlace_SwordAndShield");
            
            float jumpForce = Mathf.Sqrt(2f * jumpHeight * Physics.gravity.magnitude);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.velocity = new Vector3(rb.velocity.x, jumpHeight, rb.velocity.z);
        }
    }
    
    private void CheckGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayerMask))
        {
            isGrounded = true;
        }
        
        if (rb.velocity.y != 0f && !isGrounded)
        {
            isJumping = true;
        } else if (rb.velocity.y == 0f && isGrounded)
        {
            isJumping = false;
        }
    }
    
    private IEnumerator WaitAndWalk()
    {
        CanMove = false;

        yield return new WaitForSeconds(0.5f);

        CanMove = true;
    }
    
    #endregion
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayerMask))
        {
            Gizmos.DrawLine(hit.point, hit.point + hit.normal); 
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }

}
