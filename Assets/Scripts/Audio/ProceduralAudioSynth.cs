using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProceduralAudioSynth : MonoBehaviour
{
    private ProceduralAudioSO currentPreset;
    
    // Copia local de parámetros para modulación en tiempo real
    private float currentFrequency;
    private float currentAmplitude;
    private WaveformType currentWaveform;
    private float[] currentHarmonics = new float[10];

    // ADSR Interno
    private enum AdsrStage { Idle, Attack, Decay, Sustain, Release }
    private AdsrStage stage = AdsrStage.Idle;
    private float envelopeValue = 0f;
    private float releaseStartValue = 0f;
    private float currentSustainTimer = 0f;
    private bool isPlaying = false;

    private float sampleRate;
    private float timeIndex = 0f;

    void Awake()
    {
        sampleRate = AudioSettings.outputSampleRate;
    }

    /// <summary>
    /// Carga los parámetros del Preset y dispara la nota.
    /// </summary>
    public void PlayPreset(ProceduralAudioSO preset, float frequencyOverride = -1f)
    {
        if (preset == null) return;

        currentPreset = preset;
        currentFrequency = (frequencyOverride > 0) ? frequencyOverride : preset.baseFrequency;
        currentAmplitude = preset.amplitude;
        currentWaveform = preset.waveform;
        
        if (preset.harmonicLevels != null && preset.harmonicLevels.Length == 10)
            System.Array.Copy(preset.harmonicLevels, currentHarmonics, 10);

        // Iniciar ADSR
        isPlaying = true;
        stage = AdsrStage.Attack;
        envelopeValue = 0f;
        currentSustainTimer = preset.sustainTime / 1000f;
    }

    /// <summary>
    /// Modifica la frecuencia en vivo (útil para NodeTransit / rotaciones).
    /// </summary>
    public void SetLiveFrequency(float freq)
    {
        currentFrequency = freq;
    }

    public void StopSound()
    {
        isPlaying = false;
        if (stage != AdsrStage.Idle)
        {
            stage = AdsrStage.Release;
            releaseStartValue = envelopeValue;
        }
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (currentPreset == null || (stage == AdsrStage.Idle && !isPlaying))
        {
            for (int i = 0; i < data.Length; i++) data[i] = 0f;
            timeIndex = 0f;
            return;
        }

        float dt = 1f / sampleRate;

        for (int i = 0; i < data.Length; i += channels)
        {
            // Generación de onda según forma configurada
            float sample = GetSample(currentWaveform, currentFrequency, timeIndex);
            float env = currentPreset.useADSR ? EvaluateAdsr(dt) : 1f;

            sample *= env * currentAmplitude;

            for (int j = 0; j < channels; j++)
            {
                data[i + j] = sample;
            }

            timeIndex += dt;
        }
    }

    private float EvaluateAdsr(float dt)
    {
        switch (stage)
        {
            case AdsrStage.Attack:
                envelopeValue += dt / Mathf.Max(0.0001f, currentPreset.attack / 1000f);
                if (envelopeValue >= 1f) { envelopeValue = 1f; stage = AdsrStage.Decay; }
                break;
            case AdsrStage.Decay:
                envelopeValue -= dt / Mathf.Max(0.0001f, currentPreset.decay / 1000f);
                if (envelopeValue <= currentPreset.sustainLevel) { envelopeValue = currentPreset.sustainLevel; stage = AdsrStage.Sustain; }
                break;
            case AdsrStage.Sustain:
                envelopeValue = currentPreset.sustainLevel;
                currentSustainTimer -= dt;
                if (currentSustainTimer <= 0f || !isPlaying)
                {
                    stage = AdsrStage.Release;
                    releaseStartValue = envelopeValue;
                }
                break;
            case AdsrStage.Release:
                envelopeValue -= dt * (releaseStartValue / Mathf.Max(0.0001f, currentPreset.release / 1000f));
                if (envelopeValue <= 0f) { envelopeValue = 0f; stage = AdsrStage.Idle; }
                break;
        }
        return envelopeValue;
    }

    private float GetSample(WaveformType type, float freq, float t)
    {
        switch (type)
        {
            case WaveformType.Sine: return Mathf.Sin(2f * Mathf.PI * freq * t);
            case WaveformType.Square: return Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t));
            case WaveformType.Triangle: return Mathf.PingPong(t * freq * 2f, 1f) * 2f - 1f;
            case WaveformType.Sawtooth: return 2f * (t * freq - Mathf.Floor(t * freq + 0.5f));
            case WaveformType.Additive:
                float val = 0f;
                for (int i = 0; i < currentHarmonics.Length; i++)
                {
                    if (currentHarmonics[i] > 0f)
                        val += currentHarmonics[i] * Mathf.Sin(2f * Mathf.PI * freq * (i + 1) * t);
                }
                return val / 2.5f;
            default: return 0f;
        }
    }
}
