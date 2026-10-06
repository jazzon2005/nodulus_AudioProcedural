using UnityEngine;

namespace View.Control
{
    /// <summary>
    /// Componente de fachada / proxy ubicado en las vistas del tablero de juego.
    /// Delega las peticiones de sonido al GameAudio principal, retornando sintetizadores
    /// activos cuando la animación (NodeTransit/GridTransit) necesite modular pitch.
    /// </summary>
    public class GameBoardAudio : MonoBehaviour
    {
        private GameAudio _gameAudio;

        private void Awake()
        {
            // Intentar obtener la referencia directa de Singleton
            _gameAudio = GameAudio.Instance;

            // Búsqueda por Tag como fallback por si la instancia aún no ha despertado
            if (_gameAudio == null)
            {
                var audioGo = GameObject.FindGameObjectWithTag("GameAudio");
                if (audioGo != null)
                {
                    _gameAudio = audioGo.GetComponent<GameAudio>();
                }
            }
        }

        /// <summary>
        /// Transmite la solicitud de reproducción de SFX a GameAudio.
        /// Retorna el sintetizador asignado para posibilitar efectos de modulación dinámica.
        /// </summary>
        public ProceduralAudioSynth Play(GameClip clip, float delay = 0f, float volume = 1f, float startTime = 0f)
        {
            if (!enabled) {
                return null;
            }

            if (_gameAudio == null)
            {
                _gameAudio = GameAudio.Instance;
            }

            return _gameAudio != null ? _gameAudio.Play(clip, delay, volume, startTime) : null;
        }

        /// <summary>
        /// Transmite la solicitud de reproducción de música ambiental a GameAudio.
        /// </summary>
        public void Play(MusicClip clip, float fadeTime = 0f, float delay = 0f, float volume = 1f, float startTime = 0f)
        {
            if (!enabled) {
                return;
            }

            if (_gameAudio == null)
            {
                _gameAudio = GameAudio.Instance;
            }

            if (_gameAudio != null)
            {
                _gameAudio.Play(clip, fadeTime, delay, volume, startTime);
            }
        }
    }
}