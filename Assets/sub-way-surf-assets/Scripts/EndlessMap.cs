using System.Collections.Generic;
using UnityEngine;

public class EndlessMap : MonoBehaviour
{
    public Transform player;
    public GameObject[] segmentPrefabs;
    public int segmentsOnScreen = 5;
    [Tooltip("Số segment giữ lại phía sau player trước khi xóa")]
    public int segmentsBehind = 2;
    [Tooltip("Fallback segment length if auto-detection fails")]
    public float defaultSegmentLength = 50f;

    private List<GameObject> activeSegments = new List<GameObject>();
    private float cachedSegmentLength = 0f;
    private float cachedPivotOffset = 0f; // bounds.min.z - pivot.z của segment (hằng số)
    private float nextSpawnZ = 0f;        // vị trí Z bounds.min của segment sẽ spawn tiếp theo
    private Vector3 spawnOrigin;
    private GameObject scenePlaceholder; // Segment_01 gốc trong scene — không Destroy

    void Start()
    {
        GameObject placeholder = GameObject.Find("Segment_01");
        if (placeholder == null)
        {
            Debug.LogError("EndlessMap: 'Segment_01' not found in scene!");
            return;
        }

        cachedSegmentLength = GetFullLength(placeholder);
        if (cachedSegmentLength <= 0f)
        {
            cachedSegmentLength = defaultSegmentLength;
            Debug.LogWarning($"EndlessMap: Could not detect segment length, using default ({defaultSegmentLength}).");
        }

        // Tính pivot offset một lần từ placeholder — dùng chung cho mọi segment
        cachedPivotOffset = GetPivotOffset(placeholder);
        // nextSpawnZ = bounds.min.z thực tế của Segment_01 — điểm bắt đầu map
        nextSpawnZ = placeholder.transform.position.z + cachedPivotOffset;
        spawnOrigin = placeholder.transform.position;

        // Giữ Segment_01 trong activeSegments — nhân vật đứng trên nó ngay lúc start
        scenePlaceholder = placeholder;
        activeSegments.Add(placeholder);
        nextSpawnZ += cachedSegmentLength; // segment tiếp theo spawn ngay sau Segment_01

        for (int i = 1; i < segmentsOnScreen; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (activeSegments.Count == 0) return;

        GameObject first = activeSegments[0];
        if (first == null) return;

        // Xóa segment khi nó đã nằm đủ xa phía sau player
        if (player.position.z > GetSegmentEndZ(first) + cachedSegmentLength * segmentsBehind)
        {
            SpawnSegment();
            DeleteSegment();
        }
    }

    void SpawnSegment()
    {
        if (segmentPrefabs == null || segmentPrefabs.Length == 0) return;

        List<GameObject> validPrefabs = new List<GameObject>();
        foreach (GameObject prefab in segmentPrefabs)
        {
            if (prefab != null)
                validPrefabs.Add(prefab);
        }

        if (validPrefabs.Count == 0)
        {
            Debug.LogError("EndlessMap: All segmentPrefabs are null or destroyed! " +
                           "Make sure you assign prefab ASSETS from the Project window, " +
                           "not scene objects from the Hierarchy.");
            return;
        }

        int randomIndex = Random.Range(0, validPrefabs.Count);

        // Spawn tạm tại origin, sau đó tính pivot offset thực của segment này
        GameObject newSegment = Instantiate(validPrefabs[randomIndex], spawnOrigin, Quaternion.identity);
        newSegment.SetActive(true);

        // Tính pivot offset thực tế của segment vừa spawn (mỗi prefab có thể khác nhau)
        float actualPivotOffset = GetPivotOffset(newSegment);
        float pivotZ = nextSpawnZ - actualPivotOffset;
        newSegment.transform.position = new Vector3(spawnOrigin.x, spawnOrigin.y, pivotZ);

        nextSpawnZ += cachedSegmentLength;

        activeSegments.Add(newSegment);
    }

    void DeleteSegment()
    {
        GameObject seg = activeSegments[0];
        activeSegments.RemoveAt(0);

        if (seg == scenePlaceholder)
            seg.SetActive(false); // scene object gốc: chỉ ẩn, không Destroy
        else
            Destroy(seg);         // clone: xóa hẳn khỏi Hierarchy
    }

    // Tính chiều dài (bounds.size.z) của segment
    float GetFullLength(GameObject segment)
    {
        Renderer[] renderers = segment.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return 0f;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds.size.z;
    }

    // Trả về vị trí Z kết thúc thực tế của segment (bounds.max.z) — dùng cho despawn check
    float GetSegmentEndZ(GameObject segment)
    {
        Renderer[] renderers = segment.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return segment.transform.position.z + cachedSegmentLength;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds.max.z;
    }

    // Trả về khoảng cách từ pivot đến bounds.min.z (= bounds.min.z - pivot.z)
    float GetPivotOffset(GameObject segment)
    {
        Renderer[] renderers = segment.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return 0f;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds.min.z - segment.transform.position.z;
    }
}