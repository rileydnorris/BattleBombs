using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private List<Text> _playerScoreTextObjects;
    private List<int> _playerScores = new List<int> { 0, 0 };

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
            _playerScores[player.GetComponent<Player>().GetPlayerNumber()] += 1;
        Debug.Log(string.Join(", ", _playerScores));
    }

    public void RemoveScore(GameObject player)
    {
        if (player && player.GetComponent<Player>())
            _playerScores[player.GetComponent<Player>().GetPlayerNumber()] -= 1;
        Debug.Log(string.Join(", ", _playerScores));
    }
}
