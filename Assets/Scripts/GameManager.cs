using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        WaveManager.instance.StopWaves(); // Add this line to stop waves and clear enemies
        gameOverPanel.SetActive(true);
    }

    public void CompleteGame() // Add this method
    {
        gameCompleted = true;
        gameRunning = false;
    }
}
