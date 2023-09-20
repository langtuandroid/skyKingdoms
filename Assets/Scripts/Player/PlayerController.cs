using System.Collections;
using Attacks;
using Cinemachine;
using Managers;
using MoreMountains.Feedbacks;
using MoreMountains.Feel;
using Service;
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
        
        #region REFERENCES
        private Animator _animator;
        private CinemachineFreeLook _camera;
        private Vector3 _movement;
        private Interaction _interaction;
        private SwordAttack _swordAttack;
        private SpecialAttack _specialAttack;
        private BounceFeedbacks _bounceFeedbacks;
        private CharacterController _characterController;
        private MyInputManager _gameInputs;
        private Jump _jump;
        private PlayerAnimator _playerAnimator;
        #endregion

        #region JUMP VARIABLES
        private bool _isJumping;
        private bool _isDoubleJump;
        #endregion
    
        #region PHYSICAL ATTACK VARIABLES

        private int _actualPhysicaAttack = 1;

        #endregion
        
        #region EQUIPMENT

        private bool _hasSwordShield;
        
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
            _animator = GetComponent<Animator>();
            _interaction = GetComponent<Interaction>();
            _swordAttack = GetComponentInChildren<SwordAttack>();
            _camera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineFreeLook>();
            _specialAttack = GetComponent<SpecialAttack>();
            _characterController = GetComponent<CharacterController>();
        
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
            _jump.Init(_characterController);
        
            //ANIMATOR
            _playerAnimator.Init(_animator);
        
            //CAMERA
            var transform1 = transform;
            _camera.Follow = transform1;
            _camera.LookAt = transform1;

            _hasSwordShield = false;
        
            CanMove = true;
        }
    
        private void Update()
        {
            if (!CanMove) return;
            
            _jump.ApplyGravity(Physics.gravity.y);
            CheckGrounded();
        }

        private void FixedUpdate()
        {
            if (!CanMove) return;
        
            FixedControls();
        }

        private void LateUpdate()
        {
            _playerAnimator.PlayAnimation(_movement, false, false);
            _playerAnimator.EndJump(_characterController.isGrounded);
            _playerAnimator.Equipment(false);
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

            Vector3 move = relativeDirection * (_speed * Time.deltaTime);
            _characterController.Move(move);

            // ROTACION
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(relativeDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region ATTACK
        private void OnAttackPerformed(InputAction.CallbackContext obj)
        {
            /*if (_actualPhysicaAttack > 4) _actualPhysicaAttack = 1;
            _swordAttack.Attack();
            _playerAnimator.PlaySwordAttack(_actualPhysicaAttack);
            _actualPhysicaAttack++;*/
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
            if (!_isJumping)
            {
                _isJumping = true;
                _isDoubleJump = false;
                _jump.JumpAction(_jumpHeight, Physics.gravity.y);
                _playerAnimator.Jump();
            }
            else if(!_isDoubleJump)
            {
                _isDoubleJump = true;
                _jump.DoubleJumpAction(_jumpHeight, Physics.gravity.y);
                _playerAnimator.DoubleJump();
            }
        }

        private void CheckGrounded()
        {
            if (_characterController.isGrounded)
            {
                _isJumping = false;
                _isDoubleJump = false;
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
            /*_bounceFeedbacks.PlayCharge();
            _specialAttack.Attack();
            _playerAnimator.SpecialAttack();*/
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
