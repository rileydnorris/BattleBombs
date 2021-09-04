using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class ObjectMovement : MonoBehaviour
{
    private CharacterController2D _characterController;
    private Vector3 _velocity = new Vector3();
    private Vector2 _moveModifier = new Vector2(1, 1);
    private bool _doJump = false;
    private bool _stopMotion = false;
    private float _verticalPower;
    private float _gravity;
    private float _horizontalPower;
    public float groundDamping;

    void Start()
    {
        _characterController = GetComponent<CharacterController2D>();
    }

    void Update()
    {
        if (!_stopMotion)
        {
            var jumpVal = GetVerticalValue();
            var moveVal = GetHorizontalValue();
            Move(moveVal, jumpVal);
        }
    }

    void Move(float moveVal, float jumpVal)
    {
        _velocity.x = moveVal;
        _velocity.y = jumpVal + _gravity * Time.deltaTime;
        _characterController.move(_velocity * Time.deltaTime);
        _velocity = _characterController.velocity;
    }

    float GetVerticalValue()
    {
        var y = 0f;
        if (_doJump)
            y = Mathf.Sqrt(2f * _verticalPower * -_gravity);
        else
            y = _velocity.y;

        _doJump = false;
        return y;
    }

    float GetHorizontalValue()
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
}
