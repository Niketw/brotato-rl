using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BrotatoAgent : Agent
{
    [SerializeField] private BrotatoAgentConfig config;
    [SerializeField] private Player player;
    [SerializeField] private WaveManager waveManager;

    private int lastWaveNumber = 0;
    private int lastHealth = 100;
    private float episodeTimer = 0f;

    public override void Initialize()
    {
        if (player == null) player = GetComponent<Player>();
        if (waveManager == null) waveManager = WaveManager.instance;
    }

    public override void OnEpisodeBegin()
    {
        // Reset episode variables
        lastWaveNumber = 0;
        lastHealth = 100;
        episodeTimer = 0f;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 8 observations:
        // [0] Player pos x (float)
        // [1] Player pos y (float)
        // [2] Player health (float, normalized)
        // [3] Wave number (float, normalized)
        // [4] Wave time (float, normalized)
        // [5] To enemy x (float)
        // [6] To enemy y (float)
        // [7] Distance to enemy (float)

        Vector2 position = transform.position;
        sensor.AddObservation(position.x);
        sensor.AddObservation(position.y);
        sensor.AddObservation(player.GetCurrentHealth() / 100f);  // Normalized health

        float maxWaves = 1f; // Should match MaxWaves in WaveManager
        sensor.AddObservation(waveManager.GetCurrentWave() / maxWaves); // Normalized wave number
        sensor.AddObservation(waveManager.GetCurrentWaveTime() / 30f);  // Normalized wave time

        var nearestEnemy = EnemyManager.Instance.GetNearestEnemy(transform.position);
        if (nearestEnemy != null)
        {
            Vector2 toEnemy = (nearestEnemy.transform.position - transform.position).normalized;
            sensor.AddObservation(toEnemy.x);
            sensor.AddObservation(toEnemy.y);
            sensor.AddObservation(Vector2.Distance(transform.position, nearestEnemy.transform.position));
        }
        else
        {
            sensor.AddObservation(0f);
            sensor.AddObservation(0f);
            sensor.AddObservation(100f);  // Large distance when no enemies
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!GameManager.instance.IsGameRunning()) return;

        // Process continuous actions for movement
        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        // Update player movement through Player component
        player.SetMovement(new Vector2(moveX, moveY));

        // Track time and potentially end episode
        episodeTimer += Time.fixedDeltaTime;
        if (episodeTimer >= config.episodeTimeout)
        {
            EndEpisode();
            return;
        }

        // Check for wave completion and assign rewards
        int currentWave = waveManager.GetCurrentWave();
        if (currentWave > lastWaveNumber)
        {
            // Calculate wave completion reward with increment
            float waveReward = config.baseWaveReward + (currentWave - 1) * config.waveRewardIncrement;
            AddReward(waveReward);
            lastWaveNumber = currentWave;
        }

        // Check for damage taken
        int currentHealth = player.GetCurrentHealth();
        if (currentHealth < lastHealth)
        {
            int damageTaken = lastHealth - currentHealth;
            AddReward(damageTaken * config.damageMultiplier);
            lastHealth = currentHealth;
        }

        // Check for death
        if (currentHealth <= 0)
        {
            AddReward(config.deathPenalty);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
