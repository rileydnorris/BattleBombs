using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShot : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private GameObject _parent;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        GetComponent<SpriteRenderer>().sortingLayerName = "Player";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject != _parent)
        {
            _rigidBody.gravityScale = 0f;
            _rigidBody.velocity = new Vector3();
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(2);
        _animator.SetBool("isImpact", true);
    }

    public void SetParent(GameObject parent)
    {
        _parent = parent;
    }
}
