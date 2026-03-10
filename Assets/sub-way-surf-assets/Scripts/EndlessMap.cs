using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EndlessMap : MonoBehaviour
{
    public Transform player;
    public GameObject[] segmentPrefabs;

    [Header("Settings")]
    public float segmentLength = 112f;
    public int segmentsOnScreen = 5;

    private List<GameObject> activeSegments = new List<GameObject>();
    private float nextSpawnZ = 0f;

    void Start()
    {
        GameObject oldPart = GameObject.Find("Segment_01");

        if (oldPart)
        {
            nextSpawnZ = oldPart.transform.position.z;
            activeSegments.Add(oldPart);
            nextSpawnZ += segmentLength;
        }

        for (int i = 0; i < segmentsOnScreen; i++)
            SpawnSegment();
    }

    void Update()
    {
        if (player.position.z > activeSegments[0].transform.position.z + segmentLength)
        {
            SpawnSegment();
            DeleteSegment();
        }
    }

    void SpawnSegment()
    {
        int index = Random.Range(0, segmentPrefabs.Length);

        Vector3 spawnPos = new Vector3(0, 0, nextSpawnZ);

        // Fix riêng cho Segment 3 (index = 2)
        if (index == 2)
        {
            spawnPos.x = 0.1f;
            spawnPos.y = -2.6f;
            
        }
        if (index == 1)
        {
            spawnPos.y = 0.25f;
            spawnPos.x = 0.14f;
        }
        if(index == 0)
        {
            spawnPos.x = 0.2f;
        }

        GameObject seg = Instantiate(
            segmentPrefabs[index],
            spawnPos,
            segmentPrefabs[index].transform.rotation
        );

        SpawnPatternSystem pattern = seg.GetComponent<SpawnPatternSystem>();
        if (pattern != null)
            pattern.GenerateSegmentContent(segmentLength);

        activeSegments.Add(seg);
        nextSpawnZ += segmentLength;

        seg.SetActive(true);
    }

    void DeleteSegment()
    {
        if (activeSegments.Count == 0) return;

        GameObject seg = activeSegments[0];
        activeSegments.RemoveAt(0);

        if (seg.scene.name != null && seg.name == "Segment_01")
        {
            seg.SetActive(false);
        }
        else
        {
            Destroy(seg);
        }
    }
}