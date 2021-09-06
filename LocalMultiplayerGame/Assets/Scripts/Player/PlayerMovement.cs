using UnityEngine;
using UnityEngine.InputSystem;
using Prime31;

public class PlayerMovement : ObjectMovement
{
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
    // private CharacterController2D _characterController;
    private Vector2 _thumbstickMovement = new Vector2();
    // private Vector3 _velocity = new Vector3(0, 0, 0);
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
    protected override bool _canJump
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

    protected override void DidStart()
    {
        base.DidStart();
        _animator = GetComponent<Animator>();
        _dustTrail = transform.Find("Dust - Trail").GetComponent<ParticleSystem>();
        _dustLanding = transform.Find("Dust - Landing").GetComponent<ParticleSystem>();
        _dustJumping = transform.Find("Dust - Jumping").GetComponent<ParticleSystem>();
        _dustTrailEmission = _dustTrail.emission;
        _obstacleLayerMask = LayerMask.GetMask("Obstacles");
        SetParameters(new Vector3(6.75f, 3.5f, -45));
    }

    protected override void WillUpdate()
    {
        HandleHangTime();
        HandleJumpBuffer();
        HandleLanding();
        _jumpDisableDelay -= Time.deltaTime;
        RotatePlayer();
    }

    void RotatePlayer()
    {
        if (_thumbstickMovement.x != 0)
        {
            transform.rotation = _thumbstickMovement.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }
    }

    protected override void Move(float horizontalVal, float verticalVal)
    {
        if (_characterController.isSideCollision && !_characterController.isGrounded && Physics2D.Raycast(transform.position, Vector2.down, ignoreWallStickDistance, _obstacleLayerMask).collider == null)
        {
            if (verticalVal != _velocity.y)
            {
                // Jump off wall if user jumped
                var jumpOffForce = _characterController.isCollisionLeft ? 20 : -20;
                base.Move(horizontalVal + jumpOffForce, verticalVal);
            }
            else if (_characterController.isCollisionLeft ? horizontalVal > 0 : horizontalVal < 0)
            {
                // Move off wall if thumbstick in opposite direction of wall
                base.Move(horizontalVal, verticalVal);
            }
            // Do nothing otherwise
        }
        else
        {
            // Bsic movement
            base.Move(horizontalVal, verticalVal);
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
        if (_doJump)
            _jumpBufferCounter = maxJumpBuffer;
        else
            _jumpBufferCounter -= Time.deltaTime;

        if (_doJump)
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

    protected override float GetVerticalValue()
    {
        var y = 0f;
        if (_canJump)
        {
            _jumpBufferCounter = 0;
            _sideJumpBufferCounter = 0;
            _jumpDisableDelay = _jumpDisableTime;
            _animator.SetBool("IsJumping", true);
            StartParticleSystem(_dustJumping);
            y = Mathf.Sqrt(2f * _verticalPower * -_gravity);
        }
        else
        {
            y = _velocity.y;
        }

        if (_characterController.isGrounded)
            _animator.SetBool("IsJumping", false);

        _doJump = false;
        return y;
    }

    protected override float GetHorizontalValue()
    {
        if (_characterController.isGrounded && _thumbstickMovement.x != 0)
            _dustTrailEmission.rateOverTime = 8f;
        else
            _dustTrailEmission.rateOverTime = 0;

        var moveValue = Mathf.Lerp(_velocity.x, _moveModifier.x * _horizontalPower, Time.deltaTime * groundDamping);
        _animator.SetFloat("Speed", moveValue);
        return Mathf.Lerp(_velocity.x, _moveModifier.x * _horizontalPower, Time.deltaTime * groundDamping);
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
        SetMoveModifier(moveValue);
    }

    public void SetJumpValue(bool doJump)
    {
        _doJump = doJump;
    }
}
