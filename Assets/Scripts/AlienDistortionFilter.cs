using UnityEngine;

[RequireComponent(typeof(AudioDistortionFilter))]
public class AlienDistortionFilter : MonoBehaviour, IAlienAudioFilter
{
    private AudioDistortionFilter filter;

    public void SetFrequency(float t)
    {
        filter.distortionLevel = 1f - t;
    }

    private void Awake()
    {
        filter = GetComponent<AudioDistortionFilter>();
    }
}
