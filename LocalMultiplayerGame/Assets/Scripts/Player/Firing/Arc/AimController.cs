using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AimController : MonoBehaviour
{
    private PlayerAnimator _animator;
    private ArcRenderer _arcRenderer;
    public float angle = 0;

    void Start()
    {
        _arcRenderer = GetComponent<ArcRenderer>();
        _animator = GetComponentInParent<PlayerAnimator>();
    }

    public void UpdateAimPosition(Vector2 aimVector)
    {
        // Round angle of thumbstick movement
        var radian = Mathf.Atan2(aimVector.x, aimVector.y) * Mathf.Rad2Deg;
        List<float> vals = new List<float>(new float[] { 0, 45, 90, 135, 180, -45, -90, -135, -180 });
        angle = -1 * vals.OrderBy(x => Mathf.Abs((long)x - radian)).First() + 90;

        _arcRenderer.SetAngle(angle);
    }

    public void SetIsVisible(bool isVisible)
    {
        _arcRenderer.gameObject.SetActive(isVisible);
        if (isVisible)
            _animator.PrepareToFire();
        else
            _animator.Fire();
    }
}
