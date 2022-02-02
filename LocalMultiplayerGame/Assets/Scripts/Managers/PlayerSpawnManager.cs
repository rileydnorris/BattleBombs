using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField]
    private Map _mapReference;
    private PlayerInputManager _playerInputManager;
    private float respawnTime = 3f;

    void Start()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        var playerOne = _playerInputManager.JoinPlayer(0);
        var playerTwo = _playerInputManager.JoinPlayer(1);
    }

    public Vector3 GetSpawnPosition()
    {
        var spawnPositions = _mapReference.GetPlayerSpawnPositions();
        var spawnPos = spawnPositions[Random.Range(0, spawnPositions.Length)];
        return new Vector3(spawnPos.x, spawnPos.y, 1);
    }

    public void PlayerDeath(GameObject player)
    {
        // TODO: Pick spawn away from other players

        StartCoroutine(RespawnPlayer(player.GetComponent<PlayerInput>().playerIndex));
        Destroy(player);
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
