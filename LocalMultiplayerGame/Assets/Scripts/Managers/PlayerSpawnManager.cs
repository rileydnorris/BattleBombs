using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField]
    private Map _mapReference;
    private PlayerInputManager _playerInputManager;
    private float respawnTime = 3f;

    void Start()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerInputManager.JoinPlayer(0);
        _playerInputManager.JoinPlayer(1);
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
        StartCoroutine(RespawnPlayer(player.GetComponent<PlayerInput>().playerIndex));
    }

    public IEnumerator RespawnPlayer(int playerIndex)
    {
        Time.timeScale = 0.3f;
        var adjustedRespawnTime = Time.timeScale * respawnTime;
        yield return new WaitForSeconds(adjustedRespawnTime);

        _playerInputManager.JoinPlayer(playerIndex);
        Time.timeScale = 1f;
    }
}
