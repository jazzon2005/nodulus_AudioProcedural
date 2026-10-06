/*using System.Collections;
using UnityEngine;

/// <summary>
/// Controlador principal de reproducción de las 5 melodías para la rúbrica de evaluación.
/// Diseñado de forma modular para conectar directamente a los botones de la interfaz UI en Unity.
/// </summary>
public class SongManager : MonoBehaviour
{
    [Header("Canales de Audio / Sintetizadores")]
    [Tooltip("Canal principal para la melodía sola.")]
    public OSC OSC1; 

    [Tooltip("Canal secundario para la segunda voz o acordes.")]
    public OSC OSC2; 

    [Tooltip("Canal grave para la línea de bajo.")]
    public OSC BASS; 

    // Referencia a la corrutina activa para evitar solapamientos
    private Coroutine activeSongCoroutine;
    private Coroutine activeOSC2Coroutine;
    private Coroutine activeBassCoroutine;

    // Diccionario para rastrear la octava actual de cada sintetizador (ya que OSC.cs no guarda la octava por defecto)
    private System.Collections.Generic.Dictionary<OSC, int> oscOctaves = new System.Collections.Generic.Dictionary<OSC, int>();

    #region Métodos Públicos (Conectar a UI Buttons)

    /// <summary>
    /// REQUISITO 1: Compás simple y máximo 1 alteración accidental.
    /// Melodía: El puente de Londres (4/4, Do Mayor)
    /// </summary>
    public void PlaySong1_Simple()
    {
        StopAllSongs();
        ConfigureOscillator(OSC1, octave: 4, waveform: 0, volume: 0.35f); 
        activeSongCoroutine = StartCoroutine(Song1_PuenteLondresRoutine());
    }

    /// <summary>
    /// REQUISITO 2: Al menos un cambio de registro entre octavas.
    /// Melodía: Una vez hubo un juez (Salto explícito de Octava 4 a Octava 5)
    /// </summary>
    public void PlaySong2_RegistroOctavas()
    {
        StopAllSongs();
        ConfigureOscillator(OSC1, octave: 4, waveform: 3, volume: 0.30f); 
        activeSongCoroutine = StartCoroutine(Song2_CambioOctavaRoutine());
    }

    /// <summary>
    /// REQUISITO 3: Uso visible de silencios explícitos.
    /// Melodía: Frère Jacques / Martinillo (Con pausas medidas entre frases)
    /// </summary>
    public void PlaySong3_Silencios()
    {
        StopAllSongs();
        ConfigureOscillator(OSC1, octave: 4, waveform: 2, volume: 0.35f); 
        activeSongCoroutine = StartCoroutine(Song3_FrereJacquesSilenciosRoutine());
    }

    /// <summary>
    /// REQUISITO 4: Duración mínima de 8 compases completos.
    /// Melodía: Un elefante se balanceaba (8 compases completos estructurados)
    /// </summary>
    public void PlaySong4_OchoCompases()
    {
        StopAllSongs();
        ConfigureOscillator(OSC1, octave: 4, waveform: 1, volume: 0.25f); 
        activeSongCoroutine = StartCoroutine(Song4_UnElefanteMonofonicoRoutine());
    }

    /// <summary>
    /// REQUISITO 5: Con acompañamiento opcional o segunda voz.
    /// Melodía: Un elefante se balanceaba (OSC1 = Melodía, OSC2 = Acordes, BASS = Bajo)
    /// </summary>
    public void PlaySong5_Polifonica()
    {
        StopAllSongs();
        // Configuración de los 3 canales
        ConfigureOscillator(OSC1, octave: 4, waveform: 0, volume: 0.35f);
        ConfigureOscillator(OSC2, octave: 4, waveform: 2, volume: 0.20f);
        ConfigureOscillator(BASS, octave: 2, waveform: 1, volume: 0.40f);

        activeSongCoroutine = StartCoroutine(Song5_LeadRoutine());
        activeOSC2Coroutine = StartCoroutine(Song5_AcompanamientoRoutine());
        activeBassCoroutine = StartCoroutine(Song5_BajoRoutine());
    }

    /// <summary>
    /// Detiene inmediatamente cualquier reproducción en curso.
    /// </summary>
    public void StopAllSongs()
    {
        if (activeSongCoroutine != null) StopCoroutine(activeSongCoroutine);
        if (activeOSC2Coroutine != null) StopCoroutine(activeOSC2Coroutine);
        if (activeBassCoroutine != null) StopCoroutine(activeBassCoroutine);

        if (OSC1 != null) OSC1.NoteOff();
        if (OSC2 != null) OSC2.NoteOff();
        if (BASS != null) BASS.NoteOff();
    }

    #endregion

    #region Corrutinas de las Melodías

    // --- MELODÍA 1: El Puente de Londres ---
    private IEnumerator Song1_PuenteLondresRoutine()
    {
        float tempo = 100f;
        float negra = 60f / tempo;
        float corchea = negra / 2f;
        float blanca = negra * 2f;

        // Frase 1: G A G F | E F G
        yield return PlayNote(OSC1, "G", negra);
        yield return PlayNote(OSC1, "A", corchea);
        yield return PlayNote(OSC1, "G", corchea);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "G", blanca);

        // Frase 2: D E F | E F G
        yield return PlayNote(OSC1, "D", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "F", blanca);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "G", blanca);

        // Frase 3: G A G F | E F G | D G E C
        yield return PlayNote(OSC1, "G", negra);
        yield return PlayNote(OSC1, "A", corchea);
        yield return PlayNote(OSC1, "G", corchea);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "G", blanca);
        yield return PlayNote(OSC1, "D", blanca);
        yield return PlayNote(OSC1, "G", blanca);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "C", blanca);
    }

    // --- MELODÍA 2: Una Vez Hubo Un Juez (Cambios de Registro) ---
    private IEnumerator Song2_CambioOctavaRoutine()
    {
        float tempo = 90f;
        float negra = 60f / tempo;
        float blanca = negra * 2f;

        // Registro Medio (Octava 4)
        SetOctave(OSC1, 4);
        yield return PlayNote(OSC1, "C", negra);
        yield return PlayNote(OSC1, "D", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "C", negra);

        // SALTO DE REGISTRO EXPLÍCITO: Octava 5 (Aguda)
        SetOctave(OSC1, 5);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "G", blanca);

        // RETORNO DE REGISTRO: Octava 4
        SetOctave(OSC1, 4);
        yield return PlayNote(OSC1, "G", negra);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "D", negra);
        yield return PlayNote(OSC1, "C", blanca);
    }

    // --- MELODÍA 3: Frère Jacques (Silencios Explícitos) ---
    private IEnumerator Song3_FrereJacquesSilenciosRoutine()
    {
        float tempo = 105f;
        float negra = 60f / tempo;
        float silencio = negra; // Duración del silencio

        // Frase 1
        yield return PlayNote(OSC1, "C", negra);
        yield return PlayNote(OSC1, "D", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "C", negra);

        // SILENCIO VISIBLE DE NEGRA 1
        yield return Rest(silencio);

        // Frase 2
        yield return PlayNote(OSC1, "C", negra);
        yield return PlayNote(OSC1, "D", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "C", negra);

        // SILENCIO VISIBLE DE NEGRA 2
        yield return Rest(silencio);

        // Frase 3
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "G", negra * 2f);

        // SILENCIO FINAL
        yield return Rest(silencio * 2f);
    }

    // --- MELODÍA 4: Un Elefante ---
    private IEnumerator Song4_UnElefanteMonofonicoRoutine()
    {
        float tempo = 110f;
        float negra = 60f / tempo;
        float corchea = negra / 2f;
        float blanca = negra * 2f;

        // Compás 1
        yield return PlayNote(OSC1, "C", negra);
        yield return PlayNote(OSC1, "D", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "C", negra);

        // Compás 2
        yield return PlayNote(OSC1, "C", negra);
        yield return PlayNote(OSC1, "D", negra);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "C", negra);

        // Compás 3
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "G", blanca);

        // Compás 4
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "F", negra);
        yield return PlayNote(OSC1, "G", blanca);

        // Compás 5
        yield return PlayNote(OSC1, "G", corchea);
        yield return PlayNote(OSC1, "A", corchea);
        yield return PlayNote(OSC1, "G", corchea);
        yield return PlayNote(OSC1, "F", corchea);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "C", negra);

        // Compás 6
        yield return PlayNote(OSC1, "G", corchea);
        yield return PlayNote(OSC1, "A", corchea);
        yield return PlayNote(OSC1, "G", corchea);
        yield return PlayNote(OSC1, "F", corchea);
        yield return PlayNote(OSC1, "E", negra);
        yield return PlayNote(OSC1, "C", negra);

        // Compás 7
        yield return PlayNote(OSC1, "C", negra);
        yield return PlayNote(OSC1, "G", negra);
        yield return PlayNote(OSC1, "C", blanca);

        // Compás 8
        yield return PlayNote(OSC1, "C", negra);
        yield return PlayNote(OSC1, "G", negra);
        yield return PlayNote(OSC1, "C", blanca);
    }

    // --- MELODÍA 5: Voz Principal + Acompañamiento + Bajo ---
    private IEnumerator Song5_LeadRoutine()
    {
        // Reutiliza la estructura melódica para OSC1 en paralelo
        yield return Song4_UnElefanteMonofonicoRoutine();
    }

    private IEnumerator Song5_AcompanamientoRoutine()
    {
        float tempo = 110f;
        float negra = 60f / tempo;
        float blanca = negra * 2f;

        // Acordes / Segundas voces en OSC2
        for (int i = 0; i < 4; i++)
        {
            yield return PlayNote(OSC2, "E", blanca);
            yield return PlayNote(OSC2, "G", blanca);
        }
    }

    private IEnumerator Song5_BajoRoutine()
    {
        float tempo = 110f;
        float negra = 60f / tempo;
        float redonda = negra * 4f;

        // Línea de bajo continuo en BASS
        for (int i = 0; i < 4; i++)
        {
            yield return PlayNote(BASS, "C", redonda);
            yield return PlayNote(BASS, "G", redonda);
        }
    }

    #endregion

    #region Helpers y Métodos de Soporte

    /// <summary>
    /// Configura los parámetros básicos del sintetizador especificado.
    /// </summary>
    private void ConfigureOscillator(OSC osc, int octave, int waveform, float volume)
    {
        if (osc == null) return;
        oscOctaves[osc] = octave; // Guarda la octava localmente
        osc.waveform = (WaveformType)waveform; // Asigna el Enum correcto
        osc.amplitude = volume; // Modifica la amplitud 
    }

    /// <summary>
    /// Cambia la octava en tiempo real de un canal.
    /// </summary>
    private void SetOctave(OSC osc, int octave)
    {
        if (osc == null) return;
        oscOctaves[osc] = octave;
    }

    /// <summary>
    /// Toca una nota en el canal seleccionado durante una duración específica.
    /// </summary>
    private IEnumerator PlayNote(OSC osc, string note, float duration)
    {
        if (osc != null) 
        {
            int currentOctave = oscOctaves.ContainsKey(osc) ? oscOctaves[osc] : 4;
            osc.frequency = GetFrequencyFromNote(note, currentOctave);
            osc.NoteOn(); // OSC.cs no recibe argumentos en NoteOn
        }
        
        yield return new WaitForSeconds(duration * 0.9f); // 90% sonido
        
        if (osc != null) osc.NoteOff(); // OSC.cs no recibe argumentos en NoteOff
        
        yield return new WaitForSeconds(duration * 0.1f); // 10% articulación/silencio
    }

    /// <summary>
    /// Calcula la frecuencia en Hz basándose en la nota y la octava. 
    /// </summary>
    private float GetFrequencyFromNote(string note, int selectedOctave)
    {
        if (note == "C") return 16.3516f * Mathf.Pow(2f, selectedOctave);
        if (note == "C#") return 17.3239f * Mathf.Pow(2f, selectedOctave);
        if (note == "D") return 18.3540f * Mathf.Pow(2f, selectedOctave);
        if (note == "D#") return 19.4454f * Mathf.Pow(2f, selectedOctave);
        if (note == "E") return 20.6017f * Mathf.Pow(2f, selectedOctave);
        if (note == "F") return 21.8268f * Mathf.Pow(2f, selectedOctave);
        if (note == "F#") return 23.1246f * Mathf.Pow(2f, selectedOctave);
        if (note == "G") return 24.4997f * Mathf.Pow(2f, selectedOctave);
        if (note == "G#") return 25.9565f * Mathf.Pow(2f, selectedOctave);
        if (note == "A") return 27.5000f * Mathf.Pow(2f, selectedOctave);
        if (note == "A#") return 29.1353f * Mathf.Pow(2f, selectedOctave);
        return 30.8677f * Mathf.Pow(2f, selectedOctave); 
    }

    /// <summary>
    /// Representa un silencio de audio explícito.
    /// </summary>
    private IEnumerator Rest(float duration)
    {
        yield return new WaitForSeconds(duration);
    }

    #endregion
}*/