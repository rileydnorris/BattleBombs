using UnityEngine;
using UnityEngine.InputSystem;
using Prime31;

public class PlayerMovement : MonoBehaviour
{
    private Animator _animator;
    private ParticleSystem _dustTrail;
    private ParticleSystem.EmissionModule _dustTrailEmission;
    private ParticleSystem _dustLanding;
    private ParticleSystem _dustJumping;
    private CharacterController2D _characterController;
    private Vector2 _thumbstickMovement = new Vector2();
    private Vector3 velocity = new Vector3(0, 0, 0);
    private float deadzone = 0.3f;
    private bool _scheduledJump = false;
    private float hangCounter = 0f;
    private float sideHangCounter = 0f;
    private float jumpBufferCounter = 0f;
    private float sideJumpBufferCounter = 0f;
    private float jumpDisableDelay = 0f;
    private float jumpDisableTime = 0.2f;
    public float gravity = -45f;
    public float groundDamping = 20f;
    public float jumpHeight = 3.5f;
    public float runSpeed = 6.75f;
    public float maxHangTime = 0.2f;
    public float maxSideHangTime = 0.1f;
    public float maxJumpBuffer = .1f;
    public float maxSideJumpBuffer = .1f;
    public float ignoreWallStickDistance;

    void Awake()
    {
        _characterController = GetComponent<CharacterController2D>();
        _animator = GetComponent<Animator>();
        _dustTrail = transform.Find("Dust - Trail").GetComponent<ParticleSystem>();
        _dustLanding = transform.Find("Dust - Landing").GetComponent<ParticleSystem>();
        _dustJumping = transform.Find("Dust - Jumping").GetComponent<ParticleSystem>();

        _dustTrailEmission = _dustTrail.emission;
    }

    void Update()
    {
        RotatePlayer();
        HandleHangTime();
        HandleJumpBuffer();
        HandleLanding();

        jumpDisableDelay -= Time.deltaTime;

        var jumpVal = GetJumpValue();
        var moveVal = GetMoveValue();

        var test = transform.position;
        test.y -= ignoreWallStickDistance;
        Debug.DrawLine(transform.position, test, Color.red);
        var vec = new Vector2(transform.position.x, transform.position.y);
        int layer_mask = LayerMask.GetMask("Obstacles");
        var t = Physics2D.Raycast(transform.position, Vector2.down, ignoreWallStickDistance, layer_mask);

        if (_characterController.isSideCollision && !_characterController.isGrounded && Physics2D.Raycast(transform.position, Vector2.down, ignoreWallStickDistance, layer_mask).collider == null)
        {
            if (jumpVal != velocity.y)
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
        velocity = _characterController.velocity;
    }

    void MovePlayer(float moveVal, float jumpVal)
    {
        velocity.x = moveVal;
        velocity.y = jumpVal;
        velocity.y += gravity * Time.deltaTime;
        _characterController.move(velocity * Time.deltaTime);
    }

    void RotatePlayer()
    {
        transform.rotation = _thumbstickMovement.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
    }

    void HandleHangTime()
    {
        // Coyote time for down & side directions
        if (_characterController.isGrounded)
            hangCounter = maxHangTime;
        else
            hangCounter -= Time.deltaTime;

        if (_characterController.isSideCollision)
            sideHangCounter = maxSideHangTime;
        else
            sideHangCounter -= Time.deltaTime;
    }

    void HandleJumpBuffer()
    {
        // Start timer when user wants to jump, and wait to see if user touches ground
        if (_scheduledJump)
            jumpBufferCounter = maxJumpBuffer;
        else
            jumpBufferCounter -= Time.deltaTime;

        if (_scheduledJump)
            sideJumpBufferCounter = maxSideJumpBuffer;
        else
            sideJumpBufferCounter -= Time.deltaTime;
    }

    void HandleLanding()
    {
        if (!_characterController.wasGroundedLastFrame && _characterController.isGrounded)
        {
            StartParticleSystem(_dustLanding);
        }
    }

    bool CanJump()
    {
        // Only jump if the user:
        // * Is touching the ground or in coyote time
        // * Has requested to jump within the last 0.X seconds
        // * Did not just start jumping
        return (hangCounter > 0 && jumpBufferCounter >= 0 && jumpDisableDelay < 0) || (sideHangCounter > 0 && sideJumpBufferCounter >= 0);
    }

    float GetJumpValue()
    {
        if (CanJump())
        {
            jumpBufferCounter = 0;
            sideJumpBufferCounter = 0;
            jumpDisableDelay = jumpDisableTime;
            _scheduledJump = false;
            _animator.SetBool("IsJumping", true);
            StartParticleSystem(_dustJumping);
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
                _dustTrailEmission.rateOverTime = 8f;
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
