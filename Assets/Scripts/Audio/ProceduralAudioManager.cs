using UnityEngine;

public class ProceduralAudioManager : MonoBehaviour
{
    public static ProceduralAudioManager Instance { get; private set; }

    [Header("Pool de Sintetizadores")]
    public int synthPoolSize = 5;
    private ProceduralAudioSynth[] synthPool;
    private int poolIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePool();
    }

    private void InitializePool()
    {
        synthPool = new ProceduralAudioSynth[synthPoolSize];
        for (int i = 0; i < synthPoolSize; i++)
        {
            GameObject go = new GameObject($"ProceduralSynth_{i}");
            go.transform.SetParent(transform);
            synthPool[i] = go.AddComponent<ProceduralAudioSynth>();
        }
    }

    public ProceduralAudioSynth PlayPreset(ProceduralAudioSO preset, float freqOverride = -1f)
    {
        if (preset == null) return null;

        ProceduralAudioSynth synth = synthPool[poolIndex];
        synth.PlayPreset(preset, freqOverride);

        poolIndex = (poolIndex + 1) % synthPoolSize;
        return synth;
    }
}