using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, HealthEnabledObject
{
    private PlayerSpawnManager _spawnManager;
    private Health _health;
    private PlayerMovement _playerMovement;
    private AimController _aimController;
    private PlayerControls _controls;
    private bool _isAiming = false;
    private Vector2 _thumbstickMovement;
    private float _deadzone = 0.3f;

    void Start()
    {
        _controls = new PlayerControls();
        _health = GetComponent<Health>();
        _playerMovement = GetComponent<PlayerMovement>();
        _aimController = GetComponentInChildren<AimController>();
        _health.Setup(2);
    }

    void Update()
    {
        HandleInput();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _thumbstickMovement = context.ReadValue<Vector2>();
    }

    public void OnTriggerPress(InputAction.CallbackContext context)
    {
        if (context.performed)
            TriggerPress(true);
        else if (context.canceled)
            TriggerPress(false);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            _playerMovement.Jump();
        else if (context.canceled)
            _playerMovement.StopJump();
    }

    void HandleInput()
    {
        if (_isAiming)
        {
            _aimController.UpdateAimPosition(_thumbstickMovement);
            _playerMovement.SetThumbstickMovement(Vector2.zero);
        }
        else if (Mathf.Abs(_thumbstickMovement.x) > _deadzone || Mathf.Abs(_thumbstickMovement.y) > _deadzone)
        {
            _playerMovement.SetThumbstickMovement(_thumbstickMovement);
        }
        else
        {
            _playerMovement.SetThumbstickMovement(Vector2.zero);
        }
    }

    void TriggerPress(bool isAiming)
    {
        _isAiming = isAiming;
        _aimController.SetIsVisible(isAiming);

        if (!isAiming)
        {
            GetComponent<FiringController>().Fire(_playerMovement.direction, transform, _aimController.angle);
        }
    }

    void OnEnable()
    {
        if (_controls != null)
            _controls.Ground.Enable();
    }

    void OnDisable()
    {
        _controls.Ground.Disable();
    }

    public void OnSpawn()
    {
        // Use spawn manager to find a spawn point
        _spawnManager = GameObject.Find("PlayerManager").GetComponent<PlayerSpawnManager>();
        transform.position = _spawnManager.GetSpawnPosition();
    }

    public void OnDeath()
    {
        _spawnManager = GameObject.Find("PlayerManager").GetComponent<PlayerSpawnManager>();
        _spawnManager.PlayerDeath(gameObject);
    }

    public int GetPlayerNumber()
    {
        return GetComponent<PlayerInput>().playerIndex;
    }
}
