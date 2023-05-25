using UnityEngine;

[CreateAssetMenu(fileName = "Config", menuName = "ScriptableObjects/Config")]
public class Config : ScriptableObject
{
    public int countParticle = 20;
    public float durationWave = 4f;
    public float delayWave = 2f;
    public float heightWave = 3f;
    public float noiseScale = 0.5f;
    public float noiseStrange = 2f;
}
