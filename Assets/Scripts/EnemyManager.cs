using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float timeBetweenSpawns = 0.5f;
    [SerializeField] float spawnRadius = 4f;  // Minimum distance from player
    float currentTimeBetweenSpawns;

    Transform enemysParent;
    private Transform playerTransform;
    
    public static EnemyManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        enemysParent = GameObject.Find("Enemies").transform;
        // Cache player transform
        var player = FindAnyObjectByType<Player>();
        if (player != null) playerTransform = player.transform;
    }

    private void Update()
    {
        if(!WaveManager.instance.WaveRunning()) return;

        currentTimeBetweenSpawns -= Time.deltaTime;

        if (currentTimeBetweenSpawns <= 0)
        {
            SpawnEnemy();
            currentTimeBetweenSpawns = timeBetweenSpawns;
        }
    }

    Vector2 RandomPosition()
    {
        Vector2 spawnPos;
        int maxAttempts = 100;
        int attempts = 0;
        do
        {
            spawnPos = new Vector2(Random.Range(-16, 16), Random.Range(-8, 8));
            attempts++;
        }
        while (playerTransform != null && Vector2.Distance(spawnPos, playerTransform.position) < spawnRadius && attempts < maxAttempts);
        return spawnPos;
    }

    void SpawnEnemy()
    {
        var e = Instantiate(enemyPrefab, RandomPosition(), Quaternion.identity);
        e.transform.SetParent(enemysParent);
    }

    public void DestroyAllEnemies()
    {
        foreach (Transform e in enemysParent)
        {
            Destroy(e.gameObject);
        }
    }

    public Enemy GetNearestEnemy(Vector2 position)
    {
        Enemy nearest = null;
        float minDistance = float.MaxValue;
        
        // Use new API to find enemies without sorting for performance
        var enemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (var enemy in enemies)
        {
            float distance = Vector2.Distance(position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = enemy;
            }
        }
        
        return nearest;
    }

    public Enemy[] GetAllEnemies()
    {
        return Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }
}
