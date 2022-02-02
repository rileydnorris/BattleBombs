using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface HealthEnabledObject
{
    void OnDeath();
}

public class Health : MonoBehaviour
{
    private float _numberOfLives;
    private HealthEnabledObject _parentObj;

    void Start()
    {
        _parentObj = GetComponent<HealthEnabledObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(int lives)
    {
        _numberOfLives = lives;
    }

    public void Damage(GameObject damageSource = null, int damageAmount = 1)
    {
        _numberOfLives -= damageAmount;
        if (_numberOfLives <= 0)
        {
            if (damageSource != gameObject)
                GameObject.Find("ScoreManager").GetComponent<ScoreManager>().AddScore(damageSource);
            else
                GameObject.Find("ScoreManager").GetComponent<ScoreManager>().RemoveScore(damageSource);
            _parentObj.OnDeath();
        }
    }
}
