using UnityEngine;

public class AlienSpawner : MonoBehaviour
{
    [SerializeField] private GameObject alienPrefab;

    public void Spawn()
    {
        Vector2 pos = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        GameObject alien = Instantiate(alienPrefab);
        alien.transform.position = pos;
    }
}
