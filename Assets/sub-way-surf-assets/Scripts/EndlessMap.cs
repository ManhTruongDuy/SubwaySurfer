using System.Collections.Generic;
using UnityEngine;

public class EndlessMap : MonoBehaviour
{
    public Transform player;
    public GameObject[] segmentPrefabs;

    public int segmentsOnScreen = 5;
    public float segmentLength = 40f;

    private float spawnZ = 0f;
    private List<GameObject> activeSegments = new List<GameObject>();

    void Start()
    {
        spawnZ = transform.position.z;

        for (int i = 0; i < segmentsOnScreen; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (player.position.z > spawnZ - (segmentsOnScreen * segmentLength))
        {
            SpawnSegment();
            DeleteSegment();
        }
    }

    void SpawnSegment()
    {
        int randomIndex = Random.Range(0, segmentPrefabs.Length);
        GameObject seg = Instantiate(
    segmentPrefabs[randomIndex],
    new Vector3(transform.position.x, 0, spawnZ),
    Quaternion.identity);

        activeSegments.Add(seg);
        spawnZ += segmentLength;
    }

    void DeleteSegment()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }
}