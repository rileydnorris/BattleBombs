using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AimController : MonoBehaviour
{
    public float forceMultiplier = 50f;
    [SerializeField]
    private PlayerMovement _playerMovement;
    [SerializeField]
    private GameObject _bombShotPrefab;
    [SerializeField]
    private GameObject _shotContainer;
    private ArcRenderer _arcRenderer;
    private float angle = 0;
    private float angleVel = 5f;
    private float straightVel = 8f;

    void Start()
    {
        _arcRenderer = GetComponent<ArcRenderer>();
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
    }

    public void Fire()
    {
        float bombShotOffset = _playerMovement.isFacingRight ? -0.5f : 0.5f;
        GameObject bombShot = Instantiate(_bombShotPrefab, new Vector3(_playerMovement.transform.position.x + bombShotOffset, _playerMovement.transform.position.y + 0.065f, 0), Quaternion.identity);
        bombShot.GetComponent<BombShot>().SetAngle(angle);
    }
}
