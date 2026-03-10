using UnityEngine;

public class MeasureTool : MonoBehaviour
{
    [ContextMenu("Measure Segment")]
    void Measure()
    {
        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            bounds.Encapsulate(r.bounds);
        }
        Debug.Log($"Chiều dài chuẩn (Z): {bounds.size.z}");
        Debug.Log($"Độ lệch Center so với Pivot: {bounds.center.z - transform.position.z}");
    }
}