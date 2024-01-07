using UnityEngine;

[RequireComponent(typeof(AudioLowPassFilter))]
public class AlienLowPassFilter : MonoBehaviour, IAlienAudioFilter
{
    [SerializeField] private Vector2 lowPassCutoffRange = new Vector2(500f, 10000f);
    [SerializeField] private AnimationCurve filterCutoffCurve;

    private AudioLowPassFilter filter;

    public void SetFrequency(float t)
    {
        t = filterCutoffCurve.Evaluate(t);
        filter.cutoffFrequency = Mathf.Lerp(lowPassCutoffRange.x, lowPassCutoffRange.y, t);
    }

    private void Awake()
    {
        filter = GetComponent<AudioLowPassFilter>();
    }
}
