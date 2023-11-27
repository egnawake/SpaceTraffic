using UnityEngine;
using System;

public class Alien : MonoBehaviour
{
    public AlienMessage message;

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
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.clip = message.audioClip;
    }

    public event Action<AlienAlignment> onAccepted;
    public event Action<AlienAlignment> onShot;
}
