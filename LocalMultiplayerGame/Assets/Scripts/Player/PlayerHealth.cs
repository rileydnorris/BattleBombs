using UnityEngine;
using UnityEngine.InputSystem;
using Prime31;

public class PlayerHealth : MonoBehaviour
{
    public Vector3 _damageKnockback;
    private CharacterController2D _characterController;

    void Awake()
    {
        _characterController = GetComponent<CharacterController2D>();
    }

    void Damage()
    {

    }
}
