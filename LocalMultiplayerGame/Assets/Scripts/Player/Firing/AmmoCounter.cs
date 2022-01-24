using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCounter : MonoBehaviour
{
    public GameObject fireballIndicatorPrefab;
    private float _indicatorSize = 0.5f;
    private float _totalWidth = 0;
    private float _startPos = 0;
    private int _numAmmo = 2;
    private float _spacing = 0.25f;
    private List<GameObject> _ammoIndicators = new List<GameObject>();

    void Start()
    {
        SetupAmmoIndicators();
    }

    void SetupAmmoIndicators()
    {
        _totalWidth = _indicatorSize * _numAmmo + (_spacing * (_numAmmo - 1));
        _startPos = (-_totalWidth / 2) + (_indicatorSize / 2);
        for (int i = 0; i < _numAmmo; i++)
        {
            var xPos = transform.position.x - (_startPos + ((_indicatorSize + _spacing) * i));
            var yPos = transform.position.y;
            GameObject indicator = Instantiate(fireballIndicatorPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity, transform);
            _ammoIndicators.Add(indicator);
        }
    }

    public void FireWithType()
    {
        // Find first indicator that isn't charging and fire it
        AmmoIndicator indicator = null;
        for (int i = _ammoIndicators.Count - 1; i >= 0; i--)
        {
            var curIndicator = _ammoIndicators[i].GetComponent<AmmoIndicator>();
            if (!curIndicator.IsCharging())
            {
                indicator = curIndicator;
                break;
            }
        }

        if (indicator != null)
        {
            indicator.Fire();
        }
    }

    public bool HasAmmo()
    {
        // Checks for any indicator that isn't charging
        for (int i = 0; i < _ammoIndicators.Count; i++)
        {
            if (!_ammoIndicators[i].GetComponent<AmmoIndicator>().IsCharging())
            {
                return true;
            }
        }
        return false;
    }
}
