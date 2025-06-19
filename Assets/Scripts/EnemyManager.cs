using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float timeBetweenSpawns = 0.5f;
    float currentTimeBetweenSpawns;

    Transform enemysParent;
    
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
        return new Vector2(Random.Range(-16, 16), Random.Range(-8, 8));
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
        
        foreach (var enemy in FindObjectsOfType<Enemy>())
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
}
