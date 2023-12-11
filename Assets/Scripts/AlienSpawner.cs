using System.Collections;
using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    [Tooltip("Min and max time to wait between spawns.")]
    [SerializeField] private Vector2 waitTime = new Vector2(0.1f, 0.5f);
    [SerializeField] private JoystickController player;
    [SerializeField] private FeedbackAudioPlayer feedbackAudioPlayer;
    [SerializeField] private Alien alienPrefab;
    [SerializeField] private AlienMessage[] messages;

    private void Spawn()
    {
        Vector2 pos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        Alien alien = Instantiate(alienPrefab);
        alien.transform.position = pos;
        alien.player = player;
        alien.frequency = Random.Range(0, 1f);

        AlienMessage msg = messages[Random.Range(0, messages.Length)];
        alien.message = msg;

        alien.onAccepted += HandleAlienAccept;
        alien.onShot += HandleAlienShoot;
    }

    private void HandleAlienAccept(AlienAlignment alignment)
    {
        StartCoroutine(ProcessAlien(AlienAlignment.Good, alignment));
    }

    private void HandleAlienShoot(AlienAlignment alignment)
    {
        StartCoroutine(ProcessAlien(AlienAlignment.Bad, alignment));
    }

    private void HandleAlienClear()
    {
        StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        float time = Random.Range(waitTime.x, waitTime.y);
        yield return new WaitForSeconds(time);
        Spawn();
    }

    private IEnumerator ProcessAlien(AlienAlignment choice, AlienAlignment alignment)
    {
        if (choice == AlienAlignment.Good)
        {
            feedbackAudioPlayer.Play(FeedbackSound.Flyby);
        }
        else if (choice == AlienAlignment.Bad)
        {
            feedbackAudioPlayer.Play(FeedbackSound.Explode);
        }
        yield return new WaitUntil(() => !feedbackAudioPlayer.IsPlaying);

        if (choice == alignment)
        {
            feedbackAudioPlayer.Play(FeedbackSound.Correct);
        }
        else
        {
            feedbackAudioPlayer.Play(FeedbackSound.Wrong);
        }
        yield return new WaitUntil(() => !feedbackAudioPlayer.IsPlaying);

        yield return WaitAndSpawn();
    }

    private void Start()
    {
        Spawn();
    }
}
