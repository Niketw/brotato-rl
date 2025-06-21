using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using System.Collections.Generic;

public class BrotatoAgent : Agent
{
    [SerializeField] private BrotatoAgentConfig config;
    [SerializeField] private Player player;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private float maxEnemyDistance = 20f;  // finite max distance for padding and clamping
    private float timeInCorner = 0f;

    private int lastWaveNumber = 0;
    private int lastHealth = 100;
    private float episodeTimer = 0f;

    // Add automatic decision requests to ensure OnActionReceived is called every step
    public override void Initialize()
    {
        if (player == null) player = GetComponent<Player>();
        if (waveManager == null) waveManager = WaveManager.instance;

        // Ensure the agent requests a decision each FixedUpdate
        var dr = GetComponent<Unity.MLAgents.DecisionRequester>();
        if (dr == null)
            dr = gameObject.AddComponent<Unity.MLAgents.DecisionRequester>();
        dr.DecisionPeriod = 1;

        // NOTE: Configure your BehaviorParameters component in the Inspector:
        // Set Action Type = Discrete, Branches = [9] for 8-direction movement + no-op
    }

    public override void OnEpisodeBegin()
    {
        // Ensure the game is running when the episode starts
        if (GameManager.instance != null)
        {
            GameManager.instance.StartGame();
        }
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

        // Use GetAllEnemies to find top 15 closest enemies and add observations
        Enemy[] allEnemies = EnemyManager.Instance.GetAllEnemies();
        // Sort enemies by distance
        var enemyList = new List<(Enemy enemy, float distance)>();
        foreach (var e in allEnemies)
        {
            float d = Vector2.Distance(transform.position, e.transform.position);
            enemyList.Add((e, d));
        }
        enemyList.Sort((a, b) => a.distance.CompareTo(b.distance));
        int maxEnemies = 30;
        float infiniteDist = maxEnemyDistance;  // use finite max distance to avoid Infinity
        for (int i = 0; i < maxEnemies; i++)
        {
            if (i < enemyList.Count)
            {
                var enemyPair = enemyList[i];
                Enemy enemy = enemyPair.enemy;
                // Observe absolute position and distance
                Vector3 ePos = enemy.transform.position;
                float dist = enemyPair.distance;
                sensor.AddObservation(ePos.x);
                sensor.AddObservation(ePos.y);
                sensor.AddObservation(dist);
            }
            else
            {
                // No enemy: pad with zeros
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(maxEnemyDistance); // Use maxEnemyDistance instead of infinity
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Removed IsGameRunning guard so the agent always applies actions during training

        // Map discrete action to 8-directional movement
        int action = actions.DiscreteActions[0];
        Vector2 move = Vector2.zero;
        switch (action)
        {
            case 0: move = Vector2.up; break;
            case 1: move = new Vector2(1, 1).normalized; break;
            case 2: move = Vector2.right; break;
            case 3: move = new Vector2(1, -1).normalized; break;
            case 4: move = Vector2.down; break;
            case 5: move = new Vector2(-1, -1).normalized; break;
            case 6: move = Vector2.left; break;
            case 7: move = new Vector2(-1, 1).normalized; break;
            case 8: move = Vector2.zero; break;  // No-op action
        }
        player.SetMovement(move);

        // Survival reward each step
        AddReward(config.stepReward * Time.fixedDeltaTime);
        // Compute radial and tangential movement rewards relative to enemies
        var allEnemies = EnemyManager.Instance.GetAllEnemies();
        Vector3 threatVector = Vector3.zero;
        const float ε = 0.001f;
        foreach (var e in allEnemies)
        {
            Vector3 delta = e.transform.position - transform.position;
            float dist = delta.magnitude;
            if (dist <= 0f) continue;
            float weight = 1f / (dist + ε);
            threatVector += delta.normalized * weight;
        }
        if (threatVector != Vector3.zero)
        {
            // capture raw strength before normalization
            float netEnemyStrength = threatVector.magnitude;
            // direction toward the “center” of nearby enemies
            Vector3 r = threatVector.normalized;
            Vector3 t = new Vector3(-r.y, r.x, 0f);
            var rb = player.GetComponent<Rigidbody2D>();
            Vector2 v = rb != null ? rb.linearVelocity : Vector2.zero;
            float vR = Vector3.Dot(v, r);
            float vT = Vector3.Dot(v, t);
            if (netEnemyStrength > config.enemyVectorThreshold)
                AddReward(config.tangentialRewardScale * Mathf.Abs(vT) * Time.fixedDeltaTime);
            AddReward(-config.radialPenaltyScale * Mathf.Max(0f, vR) * Time.fixedDeltaTime);
        }

        // Corner region penalty after grace period
        Vector2 pos2D = transform.position;
        // Determine if in any 5x5 corner box given a 40x25 world with center at (0,0)
        float halfW = config.worldWidth * 0.5f;
        float halfH = config.worldHeight * 0.5f;
        bool inCorner = Mathf.Abs(pos2D.x) > (halfW - config.cornerBoxWidth)
                        && Mathf.Abs(pos2D.y) > (halfH - config.cornerBoxHeight);
        if (inCorner)
        {
            timeInCorner += Time.deltaTime;
        }
        else
        {
            timeInCorner = 0f;
        }
        if (timeInCorner > config.cornerGracePeriod)
        {
            float excess = timeInCorner - config.cornerGracePeriod;
            AddReward(-config.cornerPenalty * excess * Time.deltaTime);
        }

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
        var discreteOut = actionsOut.DiscreteActions;
        int action = 8; // default to no-op
        // Determine directional input (arrow keys or WASD)
        bool up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        bool down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
        bool left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        bool right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        if (up && right) action = 1;
        else if (down && right) action = 3;
        else if (down && left) action = 5;
        else if (up && left) action = 7;
        else if (up) action = 0;
        else if (right) action = 2;
        else if (down) action = 4;
        else if (left) action = 6;
        discreteOut[0] = action;
    }
}
