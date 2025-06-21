using UnityEngine;

[CreateAssetMenu(fileName = "BrotatoAgentConfig", menuName = "ML/BrotatoAgentConfig")]
public class BrotatoAgentConfig : ScriptableObject
{
    [Header("Wave Rewards")]
    public float baseWaveReward = 7.5f;
    public float waveRewardIncrement = 0.5f;  // Added to baseWaveReward for each subsequent wave

    [Header("Penalties")]
    public float damageMultiplier = -0.01f;   // Multiplied by damage amount
    public float deathPenalty = -10.0f;        // Applied when agent dies

    [Header("Other Settings")]
    public float episodeTimeout = 300f;        // Maximum episode length in seconds

    [Header("Rewards")]
    public float stepReward = 0.02f;          // Reward for each second survived
    public float tangentialRewardScale = 0.4f;
    public float radialPenaltyScale = 0.4f;
    public float enemyVectorThreshold = 0.2f;

    [Header("Corner Penalty Settings")]
    public float worldWidth = 40f;       // Total world width
    public float worldHeight = 25f;      // Total world height
    public float cornerBoxWidth = 5f;    // Width of corner region
    public float cornerBoxHeight = 5f;   // Height of corner region
    public float cornerGracePeriod = 2f; // Seconds free in corner
    public float cornerPenalty = 1f;     // Penalty scale when in corner
}
