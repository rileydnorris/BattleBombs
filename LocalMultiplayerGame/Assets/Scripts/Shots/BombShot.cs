using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class BombShot : MonoBehaviour
{
    private GameObject _parent;
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private CharacterController2D _characterController;
    private PhysicsMovement _objectMovement;
    private float _angle = 0;
    private bool _isExplosionStarted = false;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _objectMovement = GetComponent<PhysicsMovement>();
        _characterController = GetComponent<CharacterController2D>();
        GetComponent<SpriteRenderer>().sortingLayerName = "Player";

        _objectMovement.SetParameters(GetVelocityForAngle(_angle));
        _objectMovement.Jump();
    }

    void Update()
    {
        if (!_isExplosionStarted && _characterController.collisionState.hasCollision())
        {
            _objectMovement.StopMotion();
            StartCoroutine(Explode());
        }
    }

    Vector3 GetVelocityForAngle(float val)
    {
        Dictionary<float, Vector3> velocities = new Dictionary<float, Vector3>()
        {
            { 0, new Vector3(24, 0.1f, -50) },      // Right
            { 45, new Vector3(16, 2.2f, -50) },     // Top-right
            { 90, new Vector3(0, 7, -50) },         // Up
            { 135, new Vector3(-16, 2.2f, -50) },   // Top-left
            { 180, new Vector3(-24, 0.1f, -50) },   // Left
            { 225, new Vector3(-16f, 0, -100) },    // Bottom-left
            { 270, new Vector3(0, 0, -150) },       // Down
            { -90, new Vector3(0, 0, -150) },       // Down
            { -45, new Vector3(16f, 0, -100) },     // Bottom-right
        };
        return velocities[val];
    }

    IEnumerator Explode()
    {
        _isExplosionStarted = true;
        yield return new WaitForSeconds(2);

        var hitColliders = Physics2D.OverlapCircleAll(transform.position, 5.0f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].name == "Player")
            {
                hitColliders[i].gameObject.GetComponent<PlayerMovement>().Knockback();
            }
        }

        _animator.SetBool("isImpact", true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 5.0f);
    }

    public void SetParent(GameObject parent)
    {
        _parent = parent;
    }

    public void SetAngle(float angle)
    {
        _angle = angle;
    }
}
