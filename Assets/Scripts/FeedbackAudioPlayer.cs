using System;
using UnityEngine;

public class FeedbackAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip wrongSound;
    [SerializeField] private AudioClip flybySound;
    [SerializeField] private AudioClip explodeSound;

    private AudioSource audioSource;

    public bool IsPlaying => audioSource.isPlaying;

    public void Play(FeedbackSound sound)
    {
        AudioClip c = sound switch
        {
            FeedbackSound.Correct => correctSound,
            FeedbackSound.Wrong => wrongSound,
            FeedbackSound.Flyby => flybySound,
            FeedbackSound.Explode => explodeSound,
            _ => throw new ArgumentException("Unknown sound")
        };

        audioSource.clip = c;
        audioSource.Play();
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
}
