using UnityEngine;

public class AudioLoopDelay : MonoBehaviour
{
    [SerializeField] private float delay = 0.4f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayDelayed(delay);
        }
    }
}
