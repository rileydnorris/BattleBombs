using UnityEngine;
using UnityEngine.InputSystem;
using Prime31;

public class PlayerMovement : MonoBehaviour
{
    [Header("Gameplay Variables")]
    public float gravity = -45f;
    public float groundDamping = 20f;
    public float jumpHeight = 3.5f;
    public float runSpeed = 6.75f;
    [Header("Jump Assistance")]
    public float maxHangTime = 0.2f;
    public float maxSideHangTime = 0.1f;
    public float maxJumpBuffer = .1f;
    public float maxSideJumpBuffer = .1f;
    public float ignoreWallStickDistance;
    private Animator _animator;
    private ParticleSystem _dustTrail;
    private ParticleSystem.EmissionModule _dustTrailEmission;
    private ParticleSystem _dustLanding;
    private ParticleSystem _dustJumping;
    private CharacterController2D _characterController;
    private Vector2 _thumbstickMovement = new Vector2();
    private Vector3 _velocity = new Vector3(0, 0, 0);
    private bool _scheduledJump = false;
    private float _hangCounter = 0f;
    private float _sideHangCounter = 0f;
    private float _jumpBufferCounter = 0f;
    private float _sideJumpBufferCounter = 0f;
    private float _jumpDisableDelay = 0f;
    private float _jumpDisableTime = 0.2f;
    private int _obstacleLayerMask;

    // Getters and Setters
    public bool isFacingRight
    {
        get { return _thumbstickMovement.x > 0; }
    }
    private bool _canJump
    {
        get
        {
            // Only jump if the user:
            // * Is touching the ground or in coyote time
            // * Has requested to jump within the last 0.X seconds
            // * Did not just start jumping
            return (_hangCounter > 0 && _jumpBufferCounter >= 0 && _jumpDisableDelay < 0) || (_sideHangCounter > 0 && _sideJumpBufferCounter >= 0);

        }
    }

    void Awake()
    {
        _characterController = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
        _dustTrail = transform.Find("Dust - Trail").GetComponent<ParticleSystem>();
        _dustLanding = transform.Find("Dust - Landing").GetComponent<ParticleSystem>();
        _dustJumping = transform.Find("Dust - Jumping").GetComponent<ParticleSystem>();
        _dustTrailEmission = _dustTrail.emission;
        _obstacleLayerMask = LayerMask.GetMask("Obstacles");
    }

    void Update()
    {
        RotatePlayer();
        HandleHangTime();
        HandleJumpBuffer();
        HandleLanding();

        _jumpDisableDelay -= Time.deltaTime;

        var jumpVal = GetJumpValue();
        var moveVal = GetMoveValue();

        if (_characterController.isSideCollision && !_characterController.isGrounded && Physics2D.Raycast(transform.position, Vector2.down, ignoreWallStickDistance, _obstacleLayerMask).collider == null)
        {
            if (jumpVal != _velocity.y)
            {
                // Jump off wall if user jumped
                var jumpOffForce = _characterController.isCollisionLeft ? 20 : -20;
                MovePlayer(moveVal + jumpOffForce, jumpVal);
            }
            else if (_characterController.isCollisionLeft ? moveVal > 0 : moveVal < 0)
            {
                // Move off wall if thumbstick in opposite direction of wall
                MovePlayer(moveVal, jumpVal);
            }
            // Do nothing otherwise
        }
        else
        {
            // Bsic movement
            MovePlayer(moveVal, jumpVal);
        }
        _velocity = _characterController.velocity;
    }

    void MovePlayer(float moveVal, float jumpVal)
    {
        _velocity.x = moveVal;
        _velocity.y = jumpVal;
        _velocity.y += gravity * Time.deltaTime;
        _characterController.move(_velocity * Time.deltaTime);
    }

    void RotatePlayer()
    {
        if (_thumbstickMovement.x != 0)
        {
            transform.rotation = _thumbstickMovement.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
    }

    void HandleHangTime()
    {
        // Coyote time for down & side directions
        if (_characterController.isGrounded)
            _hangCounter = maxHangTime;
        else
            _hangCounter -= Time.deltaTime;

        if (_characterController.isSideCollision)
            _sideHangCounter = maxSideHangTime;
        else
            _sideHangCounter -= Time.deltaTime;
    }

    void HandleJumpBuffer()
    {
        // Start timer when user wants to jump, and wait to see if user touches ground
        if (_scheduledJump)
            _jumpBufferCounter = maxJumpBuffer;
        else
            _jumpBufferCounter -= Time.deltaTime;

        if (_scheduledJump)
            _sideJumpBufferCounter = maxSideJumpBuffer;
        else
            _sideJumpBufferCounter -= Time.deltaTime;
    }

    void HandleLanding()
    {
        if (!_characterController.wasGroundedLastFrame && _characterController.isGrounded)
        {
            StartParticleSystem(_dustLanding);
        }
    }

    float GetJumpValue()
    {
        if (_canJump)
        {
            _jumpBufferCounter = 0;
            _sideJumpBufferCounter = 0;
            _jumpDisableDelay = _jumpDisableTime;
            _scheduledJump = false;
            _animator.SetBool("IsJumping", true);
            StartParticleSystem(_dustJumping);
            return Mathf.Sqrt(2f * jumpHeight * -gravity);
        }

        if (_characterController.isGrounded)
            _animator.SetBool("IsJumping", false);

        _scheduledJump = false;
        return _velocity.y;
    }

    float GetMoveValue()
    {
        if (_characterController.isGrounded && _thumbstickMovement.x != 0)
            _dustTrailEmission.rateOverTime = 8f;
        else
            _dustTrailEmission.rateOverTime = 0;

        var moveValue = Mathf.Lerp(_velocity.x, _thumbstickMovement.x * runSpeed, Time.deltaTime * groundDamping);
        _animator.SetFloat("Speed", moveValue);
        return moveValue;
    }

    void StartParticleSystem(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
        ps.Stop();
        ps.Play();
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
        // Preemptively reduce jump velocity for gentle deceleration
        if (_velocity.y > 0)
        {
            _velocity.y *= 0.5f;
        }
    }
}
