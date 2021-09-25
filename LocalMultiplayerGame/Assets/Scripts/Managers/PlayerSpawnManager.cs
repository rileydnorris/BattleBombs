using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField]
    private Map _mapReference;
    private PlayerInputManager _playerInputManager;

    void Start()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerInputManager.JoinPlayer(0);
    }

    public Vector3 GetSpawnPosition()
    {
        var spawnPositions = _mapReference.GetPlayerSpawnPositions();
        var spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
        return new Vector3(spawnPos.x, spawnPos.y, 1);
    }

    public void PlayerDeath(GameObject player)
    {
        // Start slow motion
        // Wait two seconds
        // Spawn player again
        // TODO: Add slow motion
        // TODO: Pick spawn away from other players

        Destroy(player);
        StartCoroutine(RespawnPlayer());
    }

    public IEnumerator RespawnPlayer()
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(2);

        _playerInputManager.JoinPlayer(0);
        Time.timeScale = 1f;
    }
}
