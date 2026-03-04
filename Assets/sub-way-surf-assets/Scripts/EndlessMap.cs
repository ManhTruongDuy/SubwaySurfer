using System.Collections.Generic;
using UnityEngine;

public class EndlessMap : MonoBehaviour
{
    public Transform player;
    public GameObject[] segmentPrefabs;
    public int segmentsOnScreen = 5;
    [Tooltip("Fallback segment length if auto-detection fails")]
    public float defaultSegmentLength = 50f;

    private List<GameObject> activeSegments = new List<GameObject>();
    private float cachedSegmentLength = 0f;

    void Start()
    {
        GameObject firstSegment = GameObject.Find("Segment_01");
        if (firstSegment == null)
        {
            Debug.LogError("EndlessMap: 'Segment_01' not found in scene!");
            return;
        }

        activeSegments.Add(firstSegment);

        // Cache length once — avoid recalculating every frame
        cachedSegmentLength = GetFullLength(firstSegment);
        if (cachedSegmentLength <= 0f)
        {
            cachedSegmentLength = defaultSegmentLength;
            Debug.LogWarning($"EndlessMap: Could not detect segment length, using default ({defaultSegmentLength}).");
        }

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

        // Khi player vượt qua hết segment đầu tiên
        if (player.position.z > GetSegmentEndZ(first))
        {
            SpawnSegment();
            DeleteSegment();
        }
    }

    void SpawnSegment()
    {
        if (segmentPrefabs == null || segmentPrefabs.Length == 0) return;

        // Build a valid list each spawn — guards against destroyed scene objects
        // assigned by mistake (drag from Hierarchy instead of Project window)
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

        GameObject last = activeSegments[activeSegments.Count - 1];

        // Dùng bounds.max.z thực tế của segment cuối để tránh khoảng trống
        float lastEndZ = GetSegmentEndZ(last);

        Vector3 spawnPos = new Vector3(
            last.transform.position.x,
            last.transform.position.y,
            lastEndZ
        );

        GameObject newSegment = Instantiate(validPrefabs[randomIndex], spawnPos, Quaternion.identity);

        // Nếu pivot của prefab không nằm ở Z_min, bù offset để khớp hoàn toàn
        float startOffset = GetSegmentStartOffset(newSegment);
        if (Mathf.Abs(startOffset) > 0.01f)
        {
            newSegment.transform.position = new Vector3(
                spawnPos.x,
                spawnPos.y,
                lastEndZ - startOffset
            );
        }

        activeSegments.Add(newSegment);
    }

    void DeleteSegment()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }

    // Tính chiều dài thực của toàn bộ prefab theo trục Z
    float GetFullLength(GameObject segment)
    {
        Renderer[] renderers = segment.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0f;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        return bounds.size.z;
    }

    // Trả về vị trí Z kết thúc thực tế của segment (bounds.max.z)
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

    // Trả về khoảng cách từ pivot đến điểm đầu (Z_min) của segment
    // Dùng để bù offset nếu pivot không nằm ở Z_min
    float GetSegmentStartOffset(GameObject segment)
    {
        Renderer[] renderers = segment.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return 0f;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            bounds.Encapsulate(r.bounds);

        // offset âm khi pivot nằm sau Z_min (vd: pivot ở giữa -> offset = -L/2)
        return bounds.min.z - segment.transform.position.z;
    }
}