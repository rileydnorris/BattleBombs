using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator _animator;
    private ParticleSystem _dustTrail;
    private ParticleSystem.EmissionModule _dustTrailEmission;
    private ParticleSystem _dustLanding;
    private ParticleSystem _dustJumping;

    void Start()
    {
        _animator = transform.Find("Sprite").GetComponent<Animator>();
        _dustTrail = transform.Find("Dust - Trail").GetComponent<ParticleSystem>();
        _dustLanding = transform.Find("Dust - Landing").GetComponent<ParticleSystem>();
        _dustJumping = transform.Find("Dust - Jumping").GetComponent<ParticleSystem>();
        _dustTrailEmission = _dustTrail.emission;
    }

    void StartParticleSystem(ParticleSystem ps)
    {
        ps.gameObject.SetActive(true);
        ps.Stop();
        ps.Play();
    }

    public void Walk(float moveValue, bool isGrounded)
    {
        if (isGrounded && Mathf.Abs(moveValue) > 0.1)
            _dustTrailEmission.rateOverTime = 8f;
        else
            _dustTrailEmission.rateOverTime = 0;
        _animator.SetFloat("Speed", moveValue);
    }

    public void Jump()
    {
        _animator.SetBool("IsJumping", true);
        StartParticleSystem(_dustJumping);
    }

    public void Land()
    {
        _animator.SetBool("IsJumping", false);
        StartParticleSystem(_dustLanding);
    }

    public void PrepareToFire()
    {
        _animator.SetBool("IsShooting", true);
    }

    public void Fire()
    {
        _animator.SetBool("IsShooting", false);
    }
}
