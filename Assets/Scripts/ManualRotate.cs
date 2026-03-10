using UnityEngine;

public class ManualRotate : MonoBehaviour
{
    public float rotateSpeed = 5f; // Tốc độ xoay
    private float lastMouseX;

    void OnMouseDrag()
    {
        // Tính toán độ lệch của chuột khi kéo
        float deltaX = Input.GetAxis("Mouse X");

        // Xoay nhân vật quanh trục Y dựa trên độ lệch chuột
        // Dấu trừ (-) để hướng xoay thuận theo tay di chuyển
        transform.Rotate(Vector3.up, -deltaX * rotateSpeed * 10f);
    }
}