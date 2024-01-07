using System.Collections;
using UnityEngine;
using TMPro;

public class AlienSpawner : MonoBehaviour
{
    [Tooltip("Min and max time to wait between spawns.")]
    [SerializeField] private Vector2 waitTime = new Vector2(0.1f, 0.5f);

    [SerializeField] private int scoreSuccessThreshold = 5;

    [SerializeField] private Alien alienPrefab;
    [SerializeField] private JoystickController player;
    [SerializeField] private FeedbackAudioPlayer feedbackAudioPlayer;
    [SerializeField] private TMP_Text messageText;

    private MessageSelector messageSelector;
    private int score = 0;

    private void Spawn(AlienMessage message)
    {
        Vector2 pos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        Alien alien = Instantiate(alienPrefab);
        alien.transform.position = pos;
        alien.player = player;
        alien.frequency = Random.Range(0, 1f);
        alien.message = message;

        alien.onAccepted += HandleAlienAccept;
        alien.onShot += HandleAlienShoot;

        if (messageText != null)
        {
            messageText.text = $"Message: {message.alignment} {message.difficulty}";
        }
    }

    private void HandleAlienAccept(AlienAlignment alignment)
    {
        StartCoroutine(ProcessAlien(AlienAlignment.Good, alignment));
    }

    private void HandleAlienShoot(AlienAlignment alignment)
    {
        StartCoroutine(ProcessAlien(AlienAlignment.Bad, alignment));
    }

    private IEnumerator WaitAndSpawn(AlienMessage message, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Spawn(message);
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

        Loop();
    }

    private void Loop(bool first = false)
    {
        AlienMessage message = messageSelector.Select();
        if (message == null)
        {
            // Play end of game music
            FeedbackSound endResult = score >= scoreSuccessThreshold ? FeedbackSound.EndResultGood
                : FeedbackSound.EndResultBad;

            feedbackAudioPlayer.Play(endResult);
        }
        else
        {
            float time = first ? 0 : Random.Range(waitTime.x, waitTime.y);
            StartCoroutine(WaitAndSpawn(message, time));
        }
    }

    private void Awake()
    {
        messageSelector = GetComponent<MessageSelector>();
    }

    private void Start()
    {
        Loop(true);
    }
}
