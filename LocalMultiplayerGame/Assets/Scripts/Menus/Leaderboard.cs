using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<Text> _playerScoreTextObjects;

    public void Rematch()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void SetScores(List<int> playerScores)
    {
        for (int i = 0; i < playerScores.Count; i++)
        {
            var playerNum = i + 1;
            _playerScoreTextObjects[i].text = "Player " + playerNum.ToString() + ": " + playerScores[i];
        }
    }
}
