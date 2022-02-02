using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringController : MonoBehaviour
{
    [SerializeField]
    private GameObject _fireballPrefab;
    [SerializeField]
    private AmmoCounter _ammoCounter;

    public void Fire(PlayerDirection playerDirection, Transform playerPosition, float angle)
    {
        if (_ammoCounter.HasAmmo())
        {
            float shotOffset = playerDirection == PlayerDirection.Left ? -0.5f : 0.5f;
            var posX = playerPosition.position.x + shotOffset;
            var posY = playerPosition.position.y - 0.35f;

            GameObject fireball = Instantiate(_fireballPrefab, new Vector3(posX, posY, 0), Quaternion.identity);
            fireball.GetComponent<Fireball>().SetAngle(angle);
            fireball.GetComponent<Fireball>().SetParent(gameObject);

            _ammoCounter.FireWithType();
        }
    }
}
