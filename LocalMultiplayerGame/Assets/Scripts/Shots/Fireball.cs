using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : StraightMovement
{
    public float speed;
    public float diagonalSpeedModifier;
    private GameObject _parent;
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private float _angle = 0;
    private bool _isExplosionStarted = false;
    private float explosionRadius = 1.2f;
    private List<GameObject> hitObjects = new List<GameObject>();

    protected override void DidStart()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingLayerName = "Player";

        SetParameters(GetVelocityForAngle(_angle));
        StartCoroutine(StartDetonation());
        _characterController.onTriggerEnterEvent += OnTriggerEnterEvent;
    }

    protected override void WillUpdate()
    {
        if (_isExplosionStarted)
        {
            var hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].name.Contains("Player") && !hitObjects.Contains(hitColliders[i].gameObject))
                {
                    hitObjects.Add(hitColliders[i].gameObject);
                    hitColliders[i].gameObject.GetComponent<Health>().Damage(_parent);
                }
            }
        }
    }

    Vector2 GetVelocityForAngle(float val)
    {
        var baseSpeed = speed;
        var diagonalSpeed = speed / diagonalSpeedModifier;
        Dictionary<float, Vector2> velocities = new Dictionary<float, Vector2>()
        {
            { 0, new Vector2(baseSpeed, 0) },                       // Right
            { 45, new Vector2(diagonalSpeed, diagonalSpeed) },      // Top-right
            { 90, new Vector2(0, baseSpeed) },                      // Up
            { 135, new Vector2(-diagonalSpeed, diagonalSpeed) },    // Top-left
            { 180, new Vector2(-baseSpeed, 0) },                    // Left
            { 225, new Vector2(-diagonalSpeed, -diagonalSpeed) },   // Bottom-left
            { 270, new Vector2(0, -baseSpeed) },                    // Down
            { -90, new Vector2(0, -baseSpeed) },                    // Down
            { -45, new Vector2(diagonalSpeed, -diagonalSpeed) },    // Bottom-right
        };
        return velocities[val];
    }

    IEnumerator StartDetonation()
    {
        yield return new WaitForSeconds(.5f);
        Explode();
    }

    void Explode()
    {
        if (_isExplosionStarted) return;
        StopMotion();

        _isExplosionStarted = true;
        _animator.SetBool("isImpact", true);
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawSphere(transform.position, explosionRadius);
    }

    void OnTriggerEnterEvent(Collider2D collider)
    {
        Explode();
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
