using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.MLAgents.Policies; // for BehaviorType

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] Button restartButton;

    public static GameManager instance;

    private bool gameRunning = true;
    private bool gameCompleted = false; // Add this field

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Time.timeScale = 0.2f; // Ensure time scale is normal
        restartButton.onClick.AddListener(RestartGame);
        gameRunning = true;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    public bool IsGameRunning()
    {
        return gameRunning && !gameCompleted;
    }

    public void GameOver()
    {
        gameRunning = false;
        WaveManager.instance.StopWaves();

        // Auto-restart only during agent training; otherwise show Game Over UI
        var agentBP = Object.FindFirstObjectByType<BrotatoAgent>()?.GetComponent<BehaviorParameters>();
        if (agentBP != null && agentBP.BehaviorType != BehaviorType.HeuristicOnly)
        {
            RestartGame();
        }
        else
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }
    }

    public void CompleteGame() // Add this method
    {
        gameCompleted = true;
        gameRunning = false;
    }

    public void StartGame()
    {
        // Resume game state after GameOver or episode reset
        gameCompleted = false;
        gameRunning = true;
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}
