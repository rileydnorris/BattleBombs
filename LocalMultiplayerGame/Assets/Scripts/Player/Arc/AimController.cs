using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AimController : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement _playerMovement;
    [SerializeField]
    private GameObject _bombShotPrefab;
    [SerializeField]
    private GameObject _straightShotPrefab;
    private Animator _animator;
    private ArcRenderer _arcRenderer;
    private float angle = 0;

    void Start()
    {
        _arcRenderer = GetComponent<ArcRenderer>();
        _animator = gameObject.GetComponentInParent<Animator>();
    }

    public void UpdateArcPosition(Vector2 aimVector)
    {
        // Round angle of thumbstick movement
        var radian = Mathf.Atan2(aimVector.x, aimVector.y) * Mathf.Rad2Deg;
        List<float> vals = new List<float>(new float[] { 0, 45, 90, 135, 180, -45, -90, -135, -180 });
        angle = -1 * vals.OrderBy(x => Mathf.Abs((long)x - radian)).First() + 90;

        _arcRenderer.SetAngle(angle);
    }

    public void UpdateIsArcVisible(bool isVisible)
    {
        _arcRenderer.gameObject.SetActive(isVisible);
        _animator.SetBool("IsShooting", isVisible);
    }

    public void Fire()
    {
        float shotOffset = _playerMovement.isFacingRight ? -0.5f : 0.5f;
        GameObject straightShot = Instantiate(_straightShotPrefab, new Vector3(_playerMovement.transform.position.x + shotOffset, _playerMovement.transform.position.y - 0.35f, 0), Quaternion.identity);
        straightShot.GetComponent<StraightShot>().SetAngle(angle);
    }
}
