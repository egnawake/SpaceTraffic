using UnityEngine;
using System;

public class Alien : MonoBehaviour
{
    public Vector2 lowPassCutoffRange = new Vector2(500f, 10000f);
    public AlienMessage message;
    public float frequency;
    public JoystickController player;

    private AudioLowPassFilter filter;

    public void Accept()
    {
        onAccepted?.Invoke(message.alignment);
        onCleared?.Invoke();

        Destroy(gameObject);
    }

    public void Shoot()
    {
        onShot?.Invoke(message.alignment);
        onCleared?.Invoke();

        Destroy(gameObject);
    }

    private void Start()
    {
        // Set audio clip
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = message.audioClip;

        // Set up audio filter
        filter = gameObject.AddComponent<AudioLowPassFilter>();
        UpdateAudioFilter(player.Frequency);
        player.onFrequencyChanged += UpdateAudioFilter;

        audioSource.Play();
    }

    private void UpdateAudioFilter(float knob)
    {
        float t = 1f - Mathf.Abs(frequency - knob);
        filter.cutoffFrequency = Mathf.Lerp(lowPassCutoffRange.x, lowPassCutoffRange.y, t);
    }

    private void OnDestroy()
    {
        player.onFrequencyChanged -= UpdateAudioFilter;
    }

    public event Action<AlienAlignment> onAccepted;
    public event Action<AlienAlignment> onShot;
    public event Action onCleared;
}
