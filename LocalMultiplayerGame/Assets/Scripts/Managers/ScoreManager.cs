using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// This class will manage all scoring and starting/ending the game
public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private List<Text> _playerScoreTextObjects;
    [SerializeField]
    private Canvas _leaderboard;
    [SerializeField]
    private PlayerSpawnManager _playerSpawnManager;
    private List<int> _playerScores = new List<int> { 0, 0 };
    private int _scoreToWin = 1;

    public void Update()
    {
        for (int i = 0; i < _playerScores.Count; i++)
        {
            var playerNum = i + 1;
            _playerScoreTextObjects[i].text = "Player " + playerNum.ToString() + ": " + _playerScores[i];
        }
    }

    public void AddScore(GameObject player)
    {
        if (player && player.GetComponent<Player>())
        {
            _playerScores[player.GetComponent<Player>().GetPlayerNumber()] += 1;
            if (_playerScores[player.GetComponent<Player>().GetPlayerNumber()] >= _scoreToWin)
            {
                _playerSpawnManager.DisablePlayers();
                _leaderboard.gameObject.SetActive(true);
                _leaderboard.GetComponent<Leaderboard>().SetScores(_playerScores);
            }
        }
    }

    public void RemoveScore(GameObject player)
    {
        if (player && player.GetComponent<Player>())
            _playerScores[player.GetComponent<Player>().GetPlayerNumber()] -= 1;
    }
}
