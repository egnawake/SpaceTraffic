using UnityEngine;
using System;

public class Alien : MonoBehaviour
{
    public AlienMessage message;
    public float frequency;
    public JoystickController player;

    private IAlienAudioFilter[] audioFilters;

    public void Accept()
    {
        onAccepted?.Invoke(message.alignment);

        Destroy(gameObject);
    }

    public void Shoot()
    {
        onShot?.Invoke(message.alignment);

        Destroy(gameObject);
    }

    private void Start()
    {
        // Set audio clip
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = message.audioClip;

        // Set up audio filters
        audioFilters = GetComponents<IAlienAudioFilter>();
        if (audioFilters.Length > 0)
        {
            UpdateAudioFilters(player.Frequency);
            player.onFrequencyChanged += UpdateAudioFilters;
        }

        audioSource.Play();
    }

    private void UpdateAudioFilters(float knob)
    {
        float t = 1f - Mathf.Abs(frequency - knob);
        foreach (var filter in audioFilters)
        {
            filter.SetFrequency(t);
        }
    }

    private void OnDestroy()
    {
        player.onFrequencyChanged -= UpdateAudioFilters;
    }

    public event Action<AlienAlignment> onAccepted;
    public event Action<AlienAlignment> onShot;
}
