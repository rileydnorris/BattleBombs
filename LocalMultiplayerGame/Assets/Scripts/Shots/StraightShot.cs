using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightShot : ObjectMovement
{
    private GameObject _parent;
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private float _angle = 0;
    private bool _isExplosionStarted = false;
    public Vector3 test;

    protected override void DidStart()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingLayerName = "Player";

        SetParameters(GetVelocityForAngle(_angle));
        Jump();
    }

    protected override void WillUpdate()
    {
        if (!_isExplosionStarted && _characterController.collisionState.hasCollision())
        {
            StopMotion();
            StartCoroutine(Explode());
        }
    }

    Vector3 GetVelocityForAngle(float val)
    {
        Dictionary<float, Vector3> velocities = new Dictionary<float, Vector3>()
        {
            { 0, new Vector3(30, 0.4f, -30) },      // Right
            { 45, new Vector3(25, 2.5f, -40) },     // Top-right
            { 90, new Vector3(0, 7, -50) },         // Up
            { 135, new Vector3(-25, 2.5f, -40) },   // Top-left
            { 180, new Vector3(-30, 0.4f, -30) },   // Left
            { 225, new Vector3(-18f, 0, -175) },    // Bottom-left
            { 270, new Vector3(0, 0, -200) },       // Down
            { -90, new Vector3(0, 0, -200) },       // Down
            { -45, new Vector3(18f, 0, -175) },     // Bottom-right
        };
        return velocities[val];
    }

    IEnumerator Explode()
    {
        _isExplosionStarted = true;
        yield return new WaitForSeconds(2);

        var hitColliders = Physics2D.OverlapCircleAll(transform.position, 2.0f);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].name.Contains("Player"))
            {
                Debug.Log(hitColliders[i].gameObject.name);
                hitColliders[i].gameObject.GetComponent<Health>().Damage();
            }
        }

        _animator.SetBool("isImpact", true);
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(transform.position, 2.0f);
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
