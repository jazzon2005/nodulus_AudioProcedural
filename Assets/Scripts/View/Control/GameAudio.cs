using System.Collections;
using UnityEngine;

namespace View.Control
{
    /// <summary>
    /// Controlador principal de audio del juego refactorizado para síntesis procedural.
    /// Mantiene compatibilidad total con las llamadas de gameplay existentes mediante el enum GameClip,
    /// delegando la síntesis en tiempo real a ProceduralAudioManager a través de ScriptableObjects.
    /// </summary>
    public class GameAudio : MonoBehaviour
    {
        private const string MusicStatusKey = "music.status";
        private const string SfxStatusKey = "sfx.status";
        private const float MusicVolume = 0.2f;

        /// <summary>
        /// Acceso global Singleton para acceso directo sin romper la búsqueda previa por Tag.
        /// </summary>
        public static GameAudio Instance { get; private set; }

        [Header("Presets Procedurales (Reemplazan a SfxClips .wav)")]
        [Tooltip("Array que mapea directamente con la posición de cada enum GameClip.")]
        public ProceduralAudioSO[] SfxPresets;

        [Header("Música de Fondo Estática")]
        public AudioClip[] MusicClips;

        private AudioSource _musicSource;
        private int _musicVolumeTweenId;

        private bool _musicEnabled = true;
        public bool MusicEnabled
        {
            get => _musicEnabled;
            set {
                if (_musicEnabled != value) {
                    LeanTween.cancel(_musicVolumeTweenId);
                }
                
                _musicEnabled = value;
                if (_musicSource != null)
                {
                    _musicSource.volume = value ? MusicVolume : 0f;
                }
                PlayerPrefs.SetInt(MusicStatusKey, value ? 0 : 1);
            }
        }

        private bool _sfxEnabled = true;
        public bool SfxEnabled
        {
            get => _sfxEnabled;
            set {
                _sfxEnabled = value;
                PlayerPrefs.SetInt(SfxStatusKey, value ? 0 : 1);
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        private void Start()
        {
            StartMusic();

            if (!PlayerPrefs.HasKey(MusicStatusKey)) {
                PlayerPrefs.SetInt(MusicStatusKey, 0);
            }
            if (!PlayerPrefs.HasKey(SfxStatusKey)) {
                PlayerPrefs.SetInt(SfxStatusKey, 0);
            }

            MusicEnabled = PlayerPrefs.GetInt(MusicStatusKey) == 0;
            SfxEnabled = PlayerPrefs.GetInt(SfxStatusKey) == 0;
        }

        /// <summary>
        /// Toca un efecto de sonido mapeado según el enum GameClip usando el Preset Procedural correspondiente.
        /// Retorna la instancia de ProceduralAudioSynth activa si se requiere modulación de frecuencia en tiempo real.
        /// </summary>
        public ProceduralAudioSynth Play(GameClip clip, float delay = 0f, float volume = 1f, float startTime = 0f)
        {
            if (!enabled || !SfxEnabled) {
                return null;
            }

            int index = (int)clip;

            if (SfxPresets == null || index < 0 || index >= SfxPresets.Length || SfxPresets[index] == null)
            {
                Debug.LogWarning($"[GameAudio] No hay un ProceduralAudioSO asignado para el clip: {clip} (Índice: {index})");
                return null;
            }

            ProceduralAudioSO preset = SfxPresets[index];

            if (delay > 0f)
            {
                StartCoroutine(PlayDelayedRoutine(preset, delay, volume));
                return null;
            }

            // Reproducción inmediata mediante el pool del ProceduralAudioManager
            return ProceduralAudioManager.Instance.PlayPreset(preset);
        }

        private IEnumerator PlayDelayedRoutine(ProceduralAudioSO preset, float delay, float volume)
        {
            yield return new WaitForSeconds(delay);
            if (enabled && SfxEnabled)
            {
                ProceduralAudioManager.Instance.PlayPreset(preset);
            }
        }

        /// <summary>
        /// Reproduce música de fondo usando la API de LeanAudio.
        /// </summary>
        public void Play(MusicClip clip, float fadeTime = 0f, float delay = 0f, float volume = 1f, float startTime = 0f)
        {
            if (!enabled || !MusicEnabled) {
                return;
            }

            if (MusicClips == null || (uint)clip >= MusicClips.Length) {
                return;
            }
            
            var audioClip = MusicClips[(uint)clip];
            
            _musicSource = LeanAudio.play(audioClip, 0f, delay, true, startTime);

            _musicVolumeTweenId = LeanTween.value(0f, volume, fadeTime)
                .setDelay(delay)
                .setEase(LeanTweenType.easeInOutSine)
                .setOnUpdate(v => {
                    if (_musicSource != null)
                    {
                        _musicSource.volume = v;
                    }
                })
                .id;
        }

        private void StartMusic()
        {
            const float fadeTime = 3f;
            const float startTime = 32f;
            Play(MusicClip.Ambient02, fadeTime: fadeTime, volume: MusicVolume, startTime: startTime);
        }
    }

    /// <summary>
    /// Mapeo 1 a 1 con el orden del array SfxPresets en el Inspector.
    /// </summary>
    public enum GameClip
    {
        GameStart,
        WinBoard,
        NodeEnter,
        NodeLeave,
        MovePushHigh,
        MovePullHigh,
        MovePullMid,
        MovePushMid,
        MovePullLow,
        MovePushLow,
        ArcMoveHigh,
        NodeRotate90,
        InvalidRotate,
        MenuSelect,
        GameEnd,
        LevelEnable,
        ArcMoveMid,
        ArcMoveLow
    }

    /// <summary>
    /// Mapeo de clips de música de fondo.
    /// </summary>
    public enum MusicClip
    {
        Ambient01,
        Ambient02
    }
}