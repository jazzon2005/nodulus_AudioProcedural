using UnityEngine;

[CreateAssetMenu(fileName = "NewProceduralAudioPreset", menuName = "Audio/Procedural Audio Preset")]

public class ProceduralAudioSO : ScriptableObject
{
    [Header("Forma de Onda y Volumen")]
    public WaveformType waveform = WaveformType.Sine;
    [Range(0f, 1f)] public float amplitude = 0.3f;
    public float baseFrequency = 440f; // Frecuencia nota base en Hz

    [Header("Síntesis Aditiva (10 Armónicos)")]
    public bool useAdditive = false;
    [Range(0f, 1f)] public float[] harmonicLevels = new float[10] { 1f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };

    [Header("Envolvente ADSR (en ms)")]
    public bool useADSR = true;
    public float attack = 10f;
    public float decay = 150f;
    [Range(0f, 1f)] public float sustainLevel = 0.7f;
    public float sustainTime = 200f;
    public float release = 100f;

    [Header("Efectos Dinámicos de Gameplay")]
    [Tooltip("Permite modular el pitch dinámicamente según la velocidad/progreso de la animación")]
    public bool enablePitchModulation = false;
    public float minFrequencyMultiplier = 0.8f;
    public float maxFrequencyMultiplier = 1.5f;
}

public enum WaveformType
{
    Sine,
    Square,
    Triangle,
    Sawtooth,
    Additive
}