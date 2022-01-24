using UnityEngine;
using Prime31;

public class StraightMovement : MonoBehaviour
{
    protected CharacterController2D _characterController;
    protected Vector3 _velocity = new Vector3();
    protected float _verticalPower;
    protected float _horizontalPower;
    private bool _stopMotion = false;

    // Virtual Functions
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
        if (!_stopMotion)
        {
            _velocity.x = _horizontalPower;
            _velocity.y = _verticalPower;
            _characterController.move(_velocity * Time.deltaTime);
        }
        _velocity = _characterController.velocity;
    }

    public void SetParameters(Vector2 parameters)
    {
        _horizontalPower = parameters.x;
        _verticalPower = parameters.y;
    }

    public void StopMotion()
    {
        _stopMotion = true;
    }
}
