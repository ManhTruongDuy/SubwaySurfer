using System.Collections.Generic;
using UnityEngine;

public class EndlessMap : MonoBehaviour
{
    public Transform player;
    public GameObject[] segmentPrefabs;

    [Header("Settings")]
    public float segmentLength = 30f;
    public int segmentsOnScreen = 5;

    private List<GameObject> activeSegments = new List<GameObject>();
    private float nextSpawnZ = 0f;

    void Start()
    {
        // Xóa hoặc ẩn các object rác đang có sẵn trong scene để tránh trùng lặp
        GameObject oldPart = GameObject.Find("Segment_01");
        if (oldPart)
        {
            nextSpawnZ = oldPart.transform.position.z;
            activeSegments.Add(oldPart);
            nextSpawnZ += segmentLength;
        }

        for (int i = 0; i < segmentsOnScreen; i++) SpawnSegment();
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

        // Quan trọng: Sử dụng rotation gốc của Prefab để không bị lệch mesh
        GameObject seg = Instantiate(segmentPrefabs[index], new Vector3(0, 0, nextSpawnZ), segmentPrefabs[index].transform.rotation);

        // Gọi hệ thống spawn vật phẩm (Coin/Train)
        SpawnPatternSystem pattern = seg.GetComponent<SpawnPatternSystem>();
        if (pattern != null) pattern.GenerateSegmentContent(segmentLength);

        activeSegments.Add(seg);
        nextSpawnZ += segmentLength;
        seg.SetActive(true);
    }

    void DeleteSegment()
    {
        if (activeSegments.Count == 0) return;

        GameObject seg = activeSegments[0];
        activeSegments.RemoveAt(0);

        // Kiểm tra chính xác: Nếu là Object có sẵn trong Scene thì mới ẩn
        // Cách tốt nhất là kiểm tra xem nó có phải là prefab clone không
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