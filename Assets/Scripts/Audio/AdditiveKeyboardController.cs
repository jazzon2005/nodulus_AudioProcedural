/*using UnityEngine;
using UnityEngine.UI;

public class AdditiveKeyboardController : MonoBehaviour
{
    public OSC oscillator;
    public Front frontUI; 
    
    [Header("Teclado Normal")]
    public Button[] noteButtons;
    public string[] noteNames = 
    {
        "C", "C#", "D", "D#", "E", "F",
        "F#", "G", "G#", "A", "A#", "B"
    };
    public int octave = 3;

    [Header("Prueba de Instrumentos")]
    [Tooltip("Escribe aquí la Frecuencia Fundamental que quieres emular")]
    public float customFrequency = 220f; 
    public Button playCustomFreqButton; // Botón en UI para disparar la nota exacta

    private string lastNote = "C";
    private bool isNotePlaying = false; // Ayuda a actualizar octava en vivo

    void Start()
    {
        // Conexión de botones de teclado clásico
        for (int i = 0; i < noteButtons.Length; i++)
        {
            int index = i;
            if (noteButtons[i] != null)
            {
                noteButtons[i].onClick.AddListener(() => PlayNote(noteNames[index]));
            }
        }

        if (playCustomFreqButton != null)
        {
            playCustomFreqButton.onClick.AddListener(PlayCustomFrequency);
        }
    }

    void Update()
    {
        // Puede usar la barra espaciadora para simular presionar la tecla de emulación
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayCustomFrequency(); 
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ReleaseNote();
        }
    }

    public void PlayNote(string note) 
    {
        lastNote = note; 
        isNotePlaying = true;
        float newFreq = GetFrequencyFromOctave(note, octave);
        if (oscillator != null)
        {
            oscillator.frequency = newFreq;
            oscillator.NoteOn();
        }
    }

    public void PlayCustomFrequency()
    {
        isNotePlaying = true;
        if (oscillator != null)
        {
            oscillator.frequency = customFrequency;
            oscillator.NoteOn();
        }
    }

    public void ReleaseNote()
    {
        isNotePlaying = false;
        if (oscillator != null)
        {
            oscillator.NoteOff();
        }
    }

    // Esta función permite que si cambia la octava mientras toca, el sonido cambie inmediatamente
    public void UpdateOctaveLive()
    {
        if (isNotePlaying && oscillator != null)
        {
            oscillator.frequency = GetFrequencyFromOctave(lastNote, octave);
        }
    }

    float GetFrequencyFromOctave(string note, int selectedOctave)
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
}*/