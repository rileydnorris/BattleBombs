using UnityEngine;
using UnityEngine.InputSystem;

using Prime31;

public class PlayerInputController : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private AimController _aimController;
    private PlayerControls _controls;
    private CharacterController2D _characterController;
    private bool _isAiming = false;
    private InputAction.CallbackContext _thumbstickContext;
    public float gravity = -9.8f;
    private Vector3 velocity = new Vector3();
    public float groundDamping = 20f; // how fast do we change direction? higher means faster

    void Awake()
    {
        _controls = new PlayerControls();
        _playerMovement = GetComponent<PlayerMovement>();
        _aimController = GetComponentInChildren<AimController>();
        _characterController = GetComponent<CharacterController2D>();

        _controls.Ground.Move.performed += context => _thumbstickContext = context;
        // _controls.Ground.Move.canceled += context => _thumbstickContext = context;

        // _controls.Ground.Jump.performed += context => _playerMovement.Jump();
        _controls.Ground.Fire_Start.performed += context => TriggerPress(true);
        _controls.Ground.Fire_End.performed += context => TriggerPress(false);
    }

    void Update()
    {
        ThumbstickMovement(_thumbstickContext);
    }

    void OnEnable()
    {
        _controls.Ground.Enable();
    }

    void OnDisable()
    {
        _controls.Ground.Disable();
    }

    void ThumbstickMovement(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        var oldVelocity = _characterController.velocity;
        var movementValue = context.ReadValue<Vector2>();
        var deadzone = 0.3f;

        // This
        // var velocity = _characterController.velocity;
        float runSpeed = 5f;

        // Debug.Log(movementValue);
        // if (Mathf.Abs(movementValue.x) > deadzone || Mathf.Abs(movementValue.y) > deadzone)
        // {
            if (!_isAiming)
            {
                // Thumbstick moves player
                // _playerMovement.UpdateMoveInput(movementValue);
                // var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
                velocity.x = Mathf.Lerp( velocity.x, -1 * runSpeed, Time.deltaTime * groundDamping );
            }
            else
            {
                // Thumbstick aims and stops player motion
                _aimController.UpdateArcPosition(movementValue);
                // _playerMovement.UpdateMoveInput(new Vector2(0, 0));
            }
        // }
        // else
        // {
        //     // _playerMovement.UpdateMoveInput(new Vector2(0, 0));
        //     velocity.x = 0;
        // }

        // velocity.y += gravity * Time.deltaTime;

        _characterController.move(velocity * Time.deltaTime);
        velocity = _characterController.velocity;
    }

    void TriggerPress(bool isAiming)
    {
        _isAiming = isAiming;
        _aimController.UpdateIsArcVisible(isAiming);

        if (!isAiming)
        {
            _aimController.Fire();
        }
    }
}
