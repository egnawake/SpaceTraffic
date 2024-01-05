using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MessageSelector : MonoBehaviour
{
    [SerializeField] private AlienMessage[] messages;

    private IList<AlienMessage>[] buckets;

    public AlienMessage Select()
    {
        // Find first non-empty bucket
        int bucketIndex = 0;
        while (buckets[bucketIndex].Count <= 0)
        {
            bucketIndex++;
        }

        // All buckets are empty, no more messages
        if (bucketIndex >= buckets.Length)
        {
            return null;
        }

        IList<AlienMessage> b = buckets[bucketIndex];

        int messageIndex = Random.Range(0, b.Count);
        AlienMessage message = b[messageIndex];
        b.RemoveAt(messageIndex);

        return message;
    }

    private void Awake()
    {
        InitBuckets();
    }

    private void InitBuckets()
    {
        int bucketNumber = Enum.GetNames(typeof(MessageDifficulty)).Length;

        buckets = new IList<AlienMessage>[bucketNumber];
        for (int i = 0; i < bucketNumber; i++)
        {
            buckets[i] = new List<AlienMessage>();
        }

        foreach (AlienMessage m in messages)
        {
            buckets[(int)m.difficulty].Add(m);
        }
    }
}
