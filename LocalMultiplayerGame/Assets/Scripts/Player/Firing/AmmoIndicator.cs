using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoIndicator : MonoBehaviour
{
    private Renderer _renderer;
    private bool _isCharging = false;

    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    IEnumerator Recharge()
    {
        _isCharging = true;
        yield return new WaitForSeconds(2f);
        _renderer.material.color = Color.HSVToRGB(0, 0, 1);
        _isCharging = false;
    }

    public void Fire()
    {
        _renderer.material.color = Color.HSVToRGB(0, 0, .5f);
        StartCoroutine(Recharge());
    }

    public bool IsCharging()
    {
        return _isCharging;
    }
}
