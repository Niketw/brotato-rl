using System.Collections;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI timeText; // set to public for testing
    [SerializeField] TextMeshProUGUI waveText;

    public static WaveManager instance;

    public bool waveRunning = true; // set to public for testing
    int currentWave = 0;
    int currentWaveTime;
    private const int MaxWaves = 5;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartNewWave();
        timeText.text = "30";
        waveText.text = "Wave: 1";
    }

    private void Update()
    {
        // Don't update if game is not running
        if (!GameManager.instance.IsGameRunning())
            return;
    }

    public bool WaveRunning() => waveRunning && GameManager.instance.IsGameRunning();

    private void StartNewWave()
    {
        if (!GameManager.instance.IsGameRunning())
            return;

        StopAllCoroutines();
        timeText.color = Color.white;
        currentWave++;
        waveRunning = true;
        currentWaveTime = 30;
        waveText.text = "Wave: " + currentWave;
        StartCoroutine(WaveTimer());
    }

    IEnumerator WaveTimer()
    {
        while (waveRunning && GameManager.instance.IsGameRunning())
        {
            yield return new WaitForSeconds(1f);
            currentWaveTime--;

            timeText.text = currentWaveTime.ToString();

            if (currentWaveTime < 0)
            {
                WaveComplete();
            }
        }

        yield return null;
    }

    private void WaveComplete()
    {
        StopAllCoroutines();
        EnemyManager.Instance.DestroyAllEnemies();
        waveRunning = false;
        
        if (currentWave < MaxWaves && GameManager.instance.IsGameRunning())
        {
            StartCoroutine(StartNextWaveAfterDelay());
        }
        else if (currentWave >= MaxWaves)
        {
            CompleteGame();
        }
    }

    private void CompleteGame()
    {
        // Stop all game systems
        GameManager.instance.CompleteGame();
        EnemyManager.Instance.DestroyAllEnemies();
        
        // Update UI
        timeText.text = "";
        waveText.text = "All Waves Complete!";
        
        // Stop player movement via Player component
        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }       
        }
    }

    private IEnumerator StartNextWaveAfterDelay()
    {
        timeText.color = Color.yellow;
        for (int i = 5; i > 0; i--)
        {
            if (!GameManager.instance.IsGameRunning())
                yield break;

            timeText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        timeText.text = "30"; // Reset timer for next wave
        StartNewWave();
    }

    public void StopWaves()
    {
        StopAllCoroutines();
        waveRunning = false;
        EnemyManager.Instance.DestroyAllEnemies();
    }
}
