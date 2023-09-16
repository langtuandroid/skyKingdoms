using System.Collections;
using Attacks;
using Cinemachine;
using Managers;
using MoreMountains.Feedbacks;
using MoreMountains.Feel;
using Service;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
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
        private SpecialAttack _specialAttack;
        private BounceFeedbacks _bounceFeedbacks;
    
        private MyInputManager _gameInputs;
        private Jump _jump;
        private PlayerAnimator _playerAnimator;
        #endregion

        #region JUMP VARIABLES
        public float groundCheckDistance = 0.5f;
        private bool _isJumping = false;
        private int _jumpCount = 0;
        private bool _isGrounded = false;
        private bool _isDoubleJump = false;
        private bool _hasLanded = false;
        #endregion
    
        #region PHYSICAL ATTACK VARIABLES

        private int _actualPhysicaAttack = 1;

        #endregion

        public bool CanMove = false;
        private bool _isInitialized;
        #endregion
    
        private void OnDestroy()
        {
            _gameInputs.movementAction.performed -= OnMovementPerformed;
            _gameInputs.movementAction.canceled -= OnMovementCanceled;
            _gameInputs.attackAction.performed -= OnAttackPerformed;
            _gameInputs.specialAttackAction.performed -= OnSpecialAttackPerformed;
            _gameInputs.defenseAction.performed -= OnDefenseOrInteractPerformed;
            _gameInputs.jumpAction.performed -= OnJumpPerformed;
        }
    

        private void Awake()
        {
            //Components
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _interaction = GetComponent<Interaction>();
            _swordAttack = GetComponentInChildren<SwordAttack>();
            _camera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineFreeLook>();
            _specialAttack = GetComponent<SpecialAttack>();
        
            //Feel
            _bounceFeedbacks = GetComponent<BounceFeedbacks>();
            _bounceFeedbacks.ChargeFeedbacks = GameObject.Find("ChargeFeedbacks").GetComponent<MMFeedbacks>();
            _bounceFeedbacks.JumpFeedbacks = GameObject.Find("JumpFeedbacks").GetComponent<MMFeedbacks>();
            _bounceFeedbacks.LandingFeedbacks = GameObject.Find("LandingFeedbacks").GetComponent<MMFeedbacks>();
        
            //Scripts
            _jump = new Jump();
            _playerAnimator = new PlayerAnimator();
        }
    
        private void Start()
        {
            if (!_isInitialized)
                Init();
        }

        private void Init()
        {
            // INPUTS
            _gameInputs = ServiceLocator.GetService<MyInputManager>();   
            _gameInputs.movementAction.performed += OnMovementPerformed;
            _gameInputs.movementAction.canceled += OnMovementCanceled;
            _gameInputs.attackAction.performed += OnAttackPerformed;
            _gameInputs.specialAttackAction.performed += OnSpecialAttackPerformed;
            _gameInputs.defenseAction.performed += OnDefenseOrInteractPerformed;
            _gameInputs.jumpAction.performed += OnJumpPerformed;
        
            //JUMP
            _jump.Init(_rb);
        
            //ANIMATOR
            _playerAnimator.Init(_animator);
        
            //CAMERA
            var transform1 = transform;
            _camera.Follow = transform1;
            _camera.LookAt = transform1;
        
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
            if (_rb.velocity.y < -0.5f)
            {
                Vector3 downwardForce = Vector3.down * 6f;
                Vector3 forwardForce = transform.forward * 5f;
                Vector3 combinedForce = downwardForce + forwardForce;

                _rb.AddForce(combinedForce, ForceMode.Force);
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
  
            if(_jumpCount > 1) return;
            
            switch (_jumpCount)
            {
                case 0:
                    _playerAnimator.Jump();
                    _isGrounded = false;
                    _hasLanded = false;
                    _jump.JumpAction(_jumpHeight);
                    _jumpCount++;
                    break;
                case 1:
                    _playerAnimator.DoubleJump();
                    _jump.DoubleJumpAction(_jumpHeight);
                    _jumpCount++;
                    break;
            }
        }
        
        private void CheckGrounded()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f, _groundLayerMask);
            if (colliders.Length > 0)
            {
                if(!_isGrounded)
                {
                    _isGrounded = true;
                    _jumpCount = 0;
                    _playerAnimator.EndJump();
                }
            }
        }
        
        private IEnumerator WaitAndWalk()
        {
            CanMove = false;

            yield return new WaitForSeconds(0.2f);

            CanMove = true;
        }
    
        #endregion
    
    
        private void OnSpecialAttackPerformed(InputAction.CallbackContext obj)
        {
            _bounceFeedbacks.PlayCharge();
            _specialAttack.Attack();
            _playerAnimator.SpecialAttack();
        }
    
        private void OnDefenseOrInteractPerformed(InputAction.CallbackContext obj)
        {
            if(_interaction.CanInteract()) 
                _interaction.Interact();
            else 
                Debug.Log("Aun no puedo usar la defensa");
        }
    }
}
