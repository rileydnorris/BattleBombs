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
    private Vector2 _thumbstickMovement;

    void Awake()
    {
        _controls = new PlayerControls();
        _playerMovement = GetComponent<PlayerMovement>();
        _aimController = GetComponentInChildren<AimController>();
        _characterController = GetComponent<CharacterController2D>();

        _controls.Ground.Move.performed += context => _thumbstickMovement = context.ReadValue<Vector2>();
        _controls.Ground.Move.canceled += context => _thumbstickMovement = context.ReadValue<Vector2>();
        _controls.Ground.Jump.performed += context => _playerMovement.SetJumpValue(true);
        _controls.Ground.Jump.canceled += context => _playerMovement.StopJump();
        _controls.Ground.Fire_Start.performed += context => TriggerPress(true);
        _controls.Ground.Fire_End.performed += context => TriggerPress(false);
    }

    void Update()
    {
        if (_isAiming)
        {
            _aimController.UpdateArcPosition(_thumbstickMovement);
            _playerMovement.SetThumbstickMovement(Vector2.zero);
        }
        else
            _playerMovement.SetThumbstickMovement(_thumbstickMovement);
    }

    void OnEnable()
    {
        _controls.Ground.Enable();
    }

    void OnDisable()
    {
        _controls.Ground.Disable();
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
