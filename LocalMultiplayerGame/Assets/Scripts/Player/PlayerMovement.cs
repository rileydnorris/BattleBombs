using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public float jumpForce;
    public float secondaryJumpForce;
    private bool _canJump = true;
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private bool isFacingRight = true;
    private Vector2 _moveInput;
    private int _jumpCount = 0;

    private float distToGround;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        distToGround = GetComponent<CircleCollider2D>().bounds.extents.y;
    }

    void Update()
    {
        // if (_rigidBody == null || _animator == null) return;
        // HandleLanding();
        // HandleMovement();
    }

    void HandleLanding()
    {
        // Debug.Log(distToGround);
        if (Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), -Vector3.up, 0.1f) && _rigidBody.velocity.y < 0)
        {
            _jumpCount = 0;
            _animator.SetBool("IsJumping", false);
            // Debug.Log("jump count" + _jumpCount);
        }
        // var startPos = new Vector3(transform.position.x, transform.position.y - 0.6f, 0);
        // var distance = 0.1f;
        // var endPos = startPos;
        // endPos.y -= distance;
        // Debug.DrawLine(startPos, endPos, Color.blue);
    }

    public bool GetIsFacingRight()
    {
        return isFacingRight;
    }

    private void HandleMovement()
    {
        var horizontalMovement = _moveInput.x;
        var directionMultiplier = horizontalMovement < 0 ? -1.0f : 1.0f;
        if (Mathf.Abs(horizontalMovement) > 0.15)
        {
            _rigidBody.velocity = new Vector3(movementSpeed * directionMultiplier, _rigidBody.velocity.y, 0);
            _animator.SetFloat("Speed", Mathf.Abs(horizontalMovement));

            // Rotate player by direction
            transform.rotation = horizontalMovement < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
            isFacingRight = horizontalMovement > 0;
        }
        else
        {
            _rigidBody.velocity = new Vector3(0, _rigidBody.velocity.y, 0);
            _animator.SetFloat("Speed", 0);
        }
    }

    public void Jump()
    {
        if (_jumpCount == 0)
        {
            // Calculate the gravity and initial jump velocity values 
            // _jumpGravity = -(2 * JumpHeight) / Mathf.Pow(TimeToJumpHeight, 2);
            // _jumpVelocity = Mathf.Abs(_jumpGravity) * TimeToJumpHeight;

            // // Step update
            // stepMovement = (_velocity + Vector3.up * _gravity * Time.deltaTime * 0.5f) * Time.deltaTime;
            // transform.Translate(stepMovement);
            // _velocity.y += _gravity * Time.deltaTime;

            // // When jump button pressed,
            // _velocity.y = _jumpVelocity;

            _rigidBody.AddForce(new Vector2(0, jumpForce));
            _animator.SetBool("IsJumping", true);
            _jumpCount += 1;
        }
        else if (_jumpCount == 1)
        {
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
            _rigidBody.AddForce(new Vector2(0, secondaryJumpForce), ForceMode2D.Impulse);
            _animator.SetBool("IsJumping", true);
            _canJump = false;
            _jumpCount += 1;
        }
    }

    public void UpdateMoveInput(Vector2 moveInput)
    {
        _moveInput = moveInput;
    }
}
