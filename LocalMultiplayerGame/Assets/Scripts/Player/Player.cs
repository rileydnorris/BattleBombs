using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, HealthEnabledObject
{
    private PlayerSpawnManager _spawnManager;
    private Health _health;

    void Start()
    {
        _health = GetComponent<Health>();
        _health.Setup(1);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSpawn()
    {
        // Use spawn manager to find a spawn point
        _spawnManager = GameObject.Find("PlayerManager").GetComponent<PlayerSpawnManager>();
        transform.position = _spawnManager.GetSpawnPosition();
    }

    public void OnDeath()
    {
        _spawnManager = GameObject.Find("PlayerManager").GetComponent<PlayerSpawnManager>();
        _spawnManager.PlayerDeath(gameObject);
    }
}
