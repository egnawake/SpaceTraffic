using UnityEngine;

[CreateAssetMenu(fileName="New Message", menuName="Custom/Alien Message")]
public class AlienMessage : ScriptableObject
{
    public AudioClip audioClip;
    public AlienAlignment alignment;
    public MessageDifficulty difficulty;
}
