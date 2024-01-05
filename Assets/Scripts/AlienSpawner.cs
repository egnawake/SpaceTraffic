using System.Collections;
using UnityEngine;
using TMPro;

public class AlienSpawner : MonoBehaviour
{
    [Tooltip("Min and max time to wait between spawns.")]
    [SerializeField] private Vector2 waitTime = new Vector2(0.1f, 0.5f);

    [SerializeField] private Alien alienPrefab;
    [SerializeField] private JoystickController player;
    [SerializeField] private FeedbackAudioPlayer feedbackAudioPlayer;
    [SerializeField] private TMP_Text messageText;

    private MessageSelector messageSelector;
    private int score = 0;

    private void Spawn()
    {
        AlienMessage msg = ChooseMessage();
        if (msg == null)
        {
            return;
        }

        Vector2 pos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        Alien alien = Instantiate(alienPrefab);
        alien.transform.position = pos;
        alien.player = player;
        alien.frequency = Random.Range(0, 1f);
        alien.message = msg;

        alien.onAccepted += HandleAlienAccept;
        alien.onShot += HandleAlienShoot;

        if (messageText != null)
        {
            messageText.text = $"Message: {msg.alignment} {msg.difficulty}";
        }
    }

    private AlienMessage ChooseMessage()
    {
        return messageSelector.Select();
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
        // Play action sound
        if (choice == AlienAlignment.Good)
        {
            feedbackAudioPlayer.Play(FeedbackSound.Flyby);
        }
        else if (choice == AlienAlignment.Bad)
        {
            feedbackAudioPlayer.Play(FeedbackSound.Explode);
        }
        yield return new WaitUntil(() => !feedbackAudioPlayer.IsPlaying);

        // Play result sound
        if (choice == alignment)
        {
            score++;
            feedbackAudioPlayer.Play(FeedbackSound.Correct);
        }
        else
        {
            feedbackAudioPlayer.Play(FeedbackSound.Wrong);
        }
        yield return new WaitUntil(() => !feedbackAudioPlayer.IsPlaying);

        if (messageSelector.Exhausted)
        {
            // Play end of game music
        }
        else
        {
            yield return WaitAndSpawn();
        }
    }

    private void Awake()
    {
        messageSelector = GetComponent<MessageSelector>();
    }

    private void Start()
    {
        Spawn();
    }
}
