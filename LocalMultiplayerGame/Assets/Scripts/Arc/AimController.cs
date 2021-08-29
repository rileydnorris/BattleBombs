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
    private GameObject _shotContainer;
    private ArcRenderer _arcRenderer;
    private float angle = 0;
    private float angleVel = 5f;
    private float straightVel = 8f;

    Vector2 GetVelocityForAngle(float val)
    {
        Dictionary<float, Vector2> velocities = new Dictionary<float, Vector2>()
        {
            { 0, new Vector2(straightVel, 0) },
            { 45, new Vector2(angleVel, angleVel) },
            { 90, new Vector2(0, straightVel) },
            { 135, new Vector2(-angleVel, angleVel) },
            { 180, new Vector2(-straightVel, 0) },
            { 225, new Vector2(-angleVel, -angleVel) },
            { 270, new Vector2(0, -straightVel) },
            { -90, new Vector2(0, -straightVel) },
            { -45, new Vector2(angleVel, -angleVel) },
        };
        return velocities[val];
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _arcRenderer = GetComponent<ArcRenderer>();
    }

    public void UpdateArcPosition(Vector2 aimVector)
    {
        // Get angle of thumbstick movement
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
        Vector2 velocity = GetVelocityForAngle(angle);
        const float forceMultiplier = 50f;
        float bombShotOffset = _playerMovement.GetIsFacingRight() ? 0.35f : -0.35f;

        GameObject bombShot = Instantiate(_bombShotPrefab, new Vector3(_playerMovement.transform.position.x + bombShotOffset, _playerMovement.transform.position.y + 0.075f, 0), Quaternion.identity);
        bombShot.GetComponent<BombShot>().SetParent(transform.parent.gameObject);
        bombShot.transform.parent = _shotContainer.transform;
        bombShot.GetComponent<Rigidbody2D>().AddForce(new Vector2(velocity.x * forceMultiplier, velocity.y * forceMultiplier));
    }
}
