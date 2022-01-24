using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class PhysicsMovement : MonoBehaviour
{
    protected CharacterController2D _characterController;
    protected Vector3 _velocity = new Vector3();
    protected Vector2 _moveModifier = new Vector2(1, 1);
    protected bool _doJump = false;
    protected bool _stopMotion = false;
    protected float _verticalPower;
    protected float _gravity;
    protected float _horizontalPower;
    protected bool _doKnockback = false;
    public float groundDamping;

    // Virtual Functions
    protected virtual bool _canJump { get { return true; } }
    protected virtual void DidStart() { }
    protected virtual void WillUpdate() { }

    void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
        DidStart();
    }

    void Update()
    {
        WillUpdate();
        if (_doKnockback)
        {
            _doKnockback = false;
            var verticalMovement = Mathf.Sqrt(2f * 4.0f * -_gravity);
            var horizontalMovement = 30f;
            Move(horizontalMovement, verticalMovement);
        }
        else if (!_stopMotion)
        {
            Move(GetHorizontalValue(), GetVerticalValue());
        }
        _velocity = _characterController.velocity;
    }

    protected virtual void Move(float horizontalVal, float verticalVal)
    {
        _velocity.x = horizontalVal;
        _velocity.y = verticalVal + _gravity * Time.deltaTime;
        _characterController.move(_velocity * Time.deltaTime);
    }

    protected virtual float GetVerticalValue()
    {
        var y = 0f;
        if (_doJump)
            y = Mathf.Sqrt(2f * _verticalPower * -_gravity);
        else
            y = _velocity.y;

        _doJump = false;
        return y;
    }

    protected virtual float GetHorizontalValue()
    {
        return Mathf.Lerp(_velocity.x, _moveModifier.x * _horizontalPower, Time.deltaTime * groundDamping);
    }

    public void Jump()
    {
        _doJump = true;
    }

    public void StopJump()
    {
        // Preemptively reduce jump velocity for gentle deceleration
        if (_velocity.y > 0)
        {
            _velocity.y *= 0.5f;
        }
    }

    public void StopMotion()
    {
        _stopMotion = true;
    }

    public void SetMoveModifier(Vector2 moveModifier)
    {
        _moveModifier = moveModifier;
    }

    public void SetParameters(Vector3 parameters)
    {
        _horizontalPower = parameters.x;
        _verticalPower = parameters.y;
        _gravity = parameters.z;
    }

    public void Knockback()
    {
        _doKnockback = true;
    }
}
