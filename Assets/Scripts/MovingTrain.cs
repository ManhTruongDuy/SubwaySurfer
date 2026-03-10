using UnityEngine;

public class MovingTrain : MonoBehaviour
{
    public float speed = 20f; // Tốc độ chạy ngược lại

    void Update()
    {
        // Di chuyển ngược chiều (hướng về phía người chơi)
        // Dùng Vector3.back vì tàu cần chạy ngược lại trục Z của Segment
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        // Tự hủy nếu tàu chạy quá xa về phía sau để tránh tốn tài nguyên
        if (transform.localPosition.z < -20f)
        {
            Destroy(gameObject);
        }
    }
}