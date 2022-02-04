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
    private List<GameObject> _players = new List<GameObject>();

    void Start()
    {
        _playerInputManager = GetComponent<PlayerInputManager>();
        _players.Add(_playerInputManager.JoinPlayer(0).gameObject);
        _players.Add(_playerInputManager.JoinPlayer(1).gameObject);
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

        _players[playerIndex] = _playerInputManager.JoinPlayer(playerIndex).gameObject;
        Time.timeScale = 1f;
    }

    public void DisablePlayers()
    {
        _players.ForEach((player) => player.GetComponent<Player>().enabled = false);
    }
}
