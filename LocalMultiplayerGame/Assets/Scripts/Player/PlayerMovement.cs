using UnityEngine;
using UnityEngine.InputSystem;
using Prime31;

public class PlayerMovement : MonoBehaviour
{
    private Animator _animator;
    private ParticleSystem _dustTrail;
    private ParticleSystem.VelocityOverLifetimeModule _dustTrailVelocity;
    private ParticleSystem.EmissionModule _dustTrailEmission;
    private CharacterController2D _characterController;
    private Vector2 _thumbstickMovement = new Vector2();
    private Vector3 velocity = new Vector3(0, 0, 0);
    private float deadzone = 0.3f;
    private bool _scheduledJump = false;
    private float hangCounter = 0f;
    private float jumpBufferCounter = 0f;
    private float jumpDisableDelay = 0f;
    private float jumpDisableTime = 0.2f;
    public float gravity = -45f;
    public float groundDamping = 20f;
    public float jumpHeight = 3.5f;
    public float runSpeed = 6.75f;
    public float maxHangTime = 0.2f;
    public float maxJumpBuffer = .1f;

    void Awake()
    {
        _characterController = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
        _dustTrail = GetComponentInChildren<ParticleSystem>();

        _dustTrailVelocity = _dustTrail.velocityOverLifetime;
        _dustTrailEmission = _dustTrail.emission;
    }

    void Update()
    {
        RotatePlayer();
        HandleHangTime();
        HandleJumpBuffer();

        jumpDisableDelay -= Time.deltaTime;

        velocity.x = GetMoveValue();
        velocity.y = GetJumpValue();
        velocity.y += gravity * Time.deltaTime;

        _characterController.move(velocity * Time.deltaTime);
        velocity = _characterController.velocity;
    }

    void RotatePlayer()
    {
        transform.rotation = _thumbstickMovement.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        _dustTrailVelocity.x = _thumbstickMovement.x < 0 ? 0.2f : -0.2f;
    }

    void HandleHangTime()
    {
        if (_characterController.isGrounded)
            hangCounter = maxHangTime;
        else
            hangCounter -= Time.deltaTime;
    }

    void HandleJumpBuffer()
    {
        // Start timer when user wants to jump
        if (_scheduledJump)
            jumpBufferCounter = maxJumpBuffer;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    bool CanJump()
    {
        // Only jump if the user:
        // * Is touching the ground or in coyote time
        // * Has requested to jump within the last 0.X seconds
        // * Did not just start jumping
        return hangCounter > 0 && jumpBufferCounter >= 0 && jumpDisableDelay < 0;
    }

    float GetJumpValue()
    {
        if (CanJump())
        {
            jumpBufferCounter = 0;
            jumpDisableDelay = jumpDisableTime;
            _scheduledJump = false;
            _animator.SetBool("IsJumping", true);
            return Mathf.Sqrt(2f * jumpHeight * -gravity);
        }

        if (_characterController.isGrounded)
            _animator.SetBool("IsJumping", false);

        _scheduledJump = false;
        return velocity.y;
    }

    float GetMoveValue()
    {
        _dustTrailEmission.rateOverTime = 0;
        if (Mathf.Abs(_thumbstickMovement.x) > deadzone || Mathf.Abs(_thumbstickMovement.y) > deadzone)
        {
            if (_characterController.isGrounded)
                _dustTrailEmission.rateOverTime = 3f;
            var moveValue = Mathf.Lerp(velocity.x, _thumbstickMovement.x * runSpeed, Time.deltaTime * groundDamping);
            _animator.SetFloat("Speed", moveValue);
            return moveValue;
        }
        else
        {
            _animator.SetFloat("Speed", 0);
            return 0;
        }
    }

    public void SetThumbstickMovement(Vector2 moveValue)
    {
        _thumbstickMovement = moveValue;
    }

    public void SetJumpValue(bool doJump)
    {
        _scheduledJump = doJump;
    }

    public void StopJump()
    {
        if (velocity.y > 0)
        {
            velocity.y *= 0.5f;
        }
    }

    public bool GetIsFacingRight()
    {
        return _thumbstickMovement.x > 0;
    }
}
