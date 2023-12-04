using System.Collections;
using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    [SerializeField] private Alien alienPrefab;
    [SerializeField] private JoystickController player;

    [Tooltip("Min and max time to wait between spawns.")]
    [SerializeField] private Vector2 waitTime = new Vector2(0.1f, 0.5f);

    private void Spawn()
    {
        Vector2 pos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        Alien alien = Instantiate(alienPrefab);
        alien.transform.position = pos;
        alien.player = player;
        alien.frequency = Random.Range(0, 1f);

        alien.onCleared += HandleAlienClear;
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

    private void Start()
    {
        Spawn();
    }
}
