using UnityEngine;

[CreateAssetMenu(fileName = "BrotatoAgentConfig", menuName = "ML/BrotatoAgentConfig")]
public class BrotatoAgentConfig : ScriptableObject
{
    [Header("Wave Rewards")]
    public float baseWaveReward = 1.0f;
    public float waveRewardIncrement = 0.5f;  // Added to baseWaveReward for each subsequent wave

    [Header("Penalties")]
    public float damageMultiplier = -0.01f;   // Multiplied by damage amount
    public float deathPenalty = -5.0f;        // Applied when agent dies

    [Header("Other Settings")]
    public float episodeTimeout = 300f;        // Maximum episode length in seconds

    [Header("Rewards")]
    public float stepReward = 0.01f;          // Reward for each second survived
}
