using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private AimController _aimController;
    private PlayerControls _controls;
    private bool _isAiming = false;

    void Awake()
    {
        _controls = new PlayerControls();
        _playerMovement = GetComponent<PlayerMovement>();
        _aimController = GetComponentInChildren<AimController>();

        _controls.Ground.Move.performed += context => ThumbstickMovement(context);
        _controls.Ground.Move.canceled += context => ThumbstickMovement(context);

        _controls.Ground.Jump.performed += context => _playerMovement.Jump();
        _controls.Ground.Fire_Start.performed += context => TriggerPress(true);
        _controls.Ground.Fire_End.performed += context => TriggerPress(false);
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
        var movementValue = context.ReadValue<Vector2>();
        var deadzone = 0.3f;

        Debug.Log(movementValue);
        if (Mathf.Abs(movementValue.x) > deadzone || Mathf.Abs(movementValue.y) > deadzone)
        {
            if (!_isAiming)
            {
                // Thumbstick moves player
                _playerMovement.UpdateMoveInput(movementValue);
            }
            else
            {
                // Thumbstick aims and stops player motion
                _aimController.UpdateArcPosition(movementValue);
                _playerMovement.UpdateMoveInput(new Vector2(0, 0));
            }
        }
        else
        {
            _playerMovement.UpdateMoveInput(new Vector2(0, 0));
        }
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
