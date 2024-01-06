using System;
using UnityEngine;

public class FeedbackAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource flyby;
    [SerializeField] private AudioSource explode;
    [SerializeField] private AudioSource correct;
    [SerializeField] private AudioSource wrong;

    private AudioSource activeSource;

    public bool IsPlaying => activeSource == null ? false : activeSource.isPlaying;

    public void Play(FeedbackSound sound)
    {
        activeSource = sound switch
        {
            FeedbackSound.Correct => correct,
            FeedbackSound.Wrong => wrong,
            FeedbackSound.Flyby => flyby,
            FeedbackSound.Explode => explode,
            _ => throw new ArgumentException("Unknown sound")
        };

        activeSource.Play();
    }
}
