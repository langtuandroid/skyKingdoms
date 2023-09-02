using System.Collections;
using Attacks;
using Cinemachine;
using Managers;
using MoreMountains.Feedbacks;
using MoreMountains.Feel;
using Player;
using Service;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("CONFIGURATION\n")]
    [Tooltip("Velocidad del personaje.")]
    [SerializeField]
    private float _speed;

    [Tooltip("Rotación del personaje.")]
    [SerializeField]
    private float _rotationSpeed;

    [Tooltip("Fuerza del salto.")]
    [SerializeField]
    private float _jumpHeight;
    
    [Tooltip("Máscara de capa utilizada para verificar si el personaje está en el suelo.")]
    [SerializeField]
    private LayerMask _groundLayerMask;

    #region REFERENCES
    private Rigidbody _rb;
    private Animator _animator;
    private CinemachineFreeLook _camera;
    private Vector3 _movement;
    private Interaction _interaction;
    private SwordAttack _swordAttack;
    private BounceFeedbacks _bounceFeedbacks;
    
    private MyInputManager _gameInputs;
    private Jump _jump;
    private PlayerAnimator _playerAnimator;
    #endregion

    #region JUMP VARIABLES
    public float groundCheckDistance = 0.5f;
    private bool _isJumping = false;
    private bool _isGrounded = true;
    private bool _isDoubleJump = false;
    private int _jumpCount;
    #endregion
    
    #region PHYSICAL ATTACK VARIABLES

    private int _actualPhysicaAttack = 1;

    #endregion

    public bool CanMove = false;
    #endregion
    
    private void OnDestroy()
    {
        _gameInputs.movementAction.performed -= OnMovementPerformed;
        _gameInputs.movementAction.canceled -= OnMovementCanceled;
        _gameInputs.attackAction.performed -= OnAttackPerformed;
        _gameInputs.defenseAction.performed -= OnDefenseOrInteractPerformed;
        _gameInputs.jumpAction.performed -= OnJumpPerformed;
    }

    private void Awake()
    {
        //Components
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _interaction = GetComponent<Interaction>();
        _swordAttack = GetComponentInChildren<SwordAttack>();//todo diferenciar espadas
        _camera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineFreeLook>();
        
        //Feel
        _bounceFeedbacks = GetComponent<BounceFeedbacks>();
        _bounceFeedbacks.ChargeFeedbacks = GameObject.Find("ChargeFeedbacks").GetComponent<MMFeedbacks>();
        _bounceFeedbacks.JumpFeedbacks = GameObject.Find("JumpFeedbacks").GetComponent<MMFeedbacks>();
        _bounceFeedbacks.LandingFeedbacks = GameObject.Find("LandingFeedbacks").GetComponent<MMFeedbacks>();
        
        //Scripts
        _jump = new Jump();
        _playerAnimator = new PlayerAnimator();
    }
    
#if UNITY_EDITOR //todo metodo que hay que llamar desde el flujo principal
    private bool _isInitialized;
    private void Start()
    {
        if (!_isInitialized)
            Init();
    }
#endif

    public void Init()
    {
        // INPUTS
        _gameInputs = ServiceLocator.GetService<MyInputManager>();   
        _gameInputs.movementAction.performed += OnMovementPerformed;
        _gameInputs.movementAction.canceled += OnMovementCanceled;
        _gameInputs.attackAction.performed += OnAttackPerformed;
        _gameInputs.defenseAction.performed += OnDefenseOrInteractPerformed;
        _gameInputs.jumpAction.performed += OnJumpPerformed;
        
        //JUMP
        _jump.Init(_rb);
        
        //ANIMATOR
        _playerAnimator.Init(_animator);
        
        //CAMERA
        //CameraController _cam = ServiceLocator.GetService<CameraController>(); 
        //if(_cam.isActiveAndEnabled) _cam.freeLookCamera.Follow = transform;
        _camera.Follow = transform;
        _camera.LookAt = transform;

#if UNITY_EDITOR
        CanMove = true;
#endif
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
       _playerAnimator.PlayAnimation(_movement, _isJumping, _isGrounded);
    }

    #region MOVEMENT

    private void OnMovementPerformed(InputAction.CallbackContext callbackContext)
    {
        _movement = callbackContext.ReadValue<Vector2>();
    }

    private void OnMovementCanceled(InputAction.CallbackContext callbackContext)
    {
        _movement = Vector2.zero;
    }

    private void FixedControls()
    {
        // Movimiento del personaje
        Vector3 direction = new Vector3(_movement.x, 0, _movement.y);

        // Calcula la dirección del movimiento relativa a la cámara
        Vector3 relativeDirection = Camera.main.transform.TransformDirection(direction);
        relativeDirection.y = 0;
        relativeDirection.Normalize();

        // MOVIMIENTO
        Vector3 move = relativeDirection * _speed * Time.deltaTime;
        _rb.MovePosition(_rb.position + move);

        // ROTACION
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(relativeDirection, Vector3.up);
            _rb.rotation = Quaternion.Slerp(_rb.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        }
        
        //CAIDA DESDE SALTO
        if (!_isGrounded && _rb.velocity.y < 0)
        {
            Debug.Log("Caigoooooooo");
            _rb.AddForce(Vector3.down * 2, ForceMode.Force);
        }
    }
    #endregion

    #region ATTACK
    private void OnAttackPerformed(InputAction.CallbackContext obj)
    {
        if (_actualPhysicaAttack > 4) _actualPhysicaAttack = 1;
        _swordAttack.Attack();
        _playerAnimator.PlaySwordAttack(_actualPhysicaAttack);
        _actualPhysicaAttack++;
    }

    //Método que llamamos desde el animator para reseteo de ataque físico
    private void ResetPhysicalAttack()
    {
        _swordAttack.ResetPhysicalAttackCollisions();
    }
    #endregion

    #region JUMP
    
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (_isGrounded && !_isJumping)
        {
            _bounceFeedbacks.PlayJump();
            _isJumping = true;
            _isDoubleJump = false;
            _playerAnimator.Jump();
            _jump.JumpAction(_jumpHeight);
        }
        else if(!_isDoubleJump)
        {
            _bounceFeedbacks.PlayCharge();
            _isDoubleJump = true;
            _playerAnimator.DoubleJump();
            _jump.DoubleJumpAction(_jumpHeight);
        }
    }

    
    private void CheckGrounded()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, _groundLayerMask))
        {
            _isGrounded = true;
        }
        
        if (_rb.velocity.y != 0f && !_isGrounded)
        {
            _isJumping = true;
        } else if (_rb.velocity.y == 0f && _isGrounded)
        {
            _isJumping = false;
        }
    }
    
    private IEnumerator WaitAndWalk()
    {
        CanMove = false;

        yield return new WaitForSeconds(0.2f);

        CanMove = true;
    }
    
    #endregion
    
    private void OnDefenseOrInteractPerformed(InputAction.CallbackContext obj)
    {
        if(_interaction.CanInteract()) 
            _interaction.Interact();
        else 
            Debug.Log("Aun no puedo usar la defensa");
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, _groundLayerMask))
        {
            Gizmos.DrawLine(hit.point, hit.point + hit.normal); 
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }
}
