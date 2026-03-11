using UnityEngine;

public class SpawnPatternSystem : MonoBehaviour
{
    public GameObject trainPrefab;
    public GameObject coinPrefab;
    public GameObject barrierLowPrefab;  // Loại nhảy qua được
    public GameObject barrierHighPrefab; // Loại phải chui qua
    public GameObject tunnelPrefab;      // Hầm (thường chiếm cả lane)
    [Header("Settings")]
    public float centerX = 0f;// là 0 nếu model đã căn giữa, hoặc điều chỉnh lại
    public float laneDistance = 2.04f;
    public float trainHeight = -0.35f;
    public float coinHeight = 0.75f;
    public float barrierLowHeight = -4.1f;    // Thường là 0 (sát đất)
    public float barrierHighHeight = 1.5f;     // Chỉnh độ cao chân rào chắn
    [Header("Spacing")]
    public float segmentLength = 30f;
    public float sectionStep = 10f;



    public void GenerateSegmentContent(float length)
    {
        segmentLength = length;
        for (float zLocal = 10f; zLocal < segmentLength - 10f; zLocal += sectionStep)
        {
            int roll = Random.Range(0, 100);

            if (roll < 30)
                SpawnTrainPattern(zLocal);
            else if (roll < 60)
                SpawnObstaclePattern(zLocal); // Pattern mới cho vật cản
            else if (roll < 85)
                SpawnCoinPattern(zLocal);
        }
    }
    void SpawnObstaclePattern(float zLocal)
    {
        int pattern = Random.Range(0, 4);
        int lane = Random.Range(0, 3);

        switch (pattern)
        {
            case 0: // Chặn 2 lane bằng rào thấp, 1 lane trống hoặc có xu
                SpawnBarrier(0, zLocal, false);
                SpawnBarrier(2, zLocal, false);
                if (Random.value > 0.5f) SpawnCoin(1, zLocal);
                break;

            case 1: // Rào cao (bắt chui) xen kẽ rào thấp (bắt nhảy)
                SpawnBarrier(lane, zLocal, true); // High
                int otherLane = (lane + 1) % 3;
                SpawnBarrier(otherLane, zLocal + 5f, false); // Low
                break;

            case 2: // Tunnel (Hầm) - Thường đi kèm với đồng xu bên trong
                SpawnTunnel(zLocal);

                // Giả sử X = 2.14 tương ứng với Lane bên phải (Lane 2)
                // Ta cho xu xuất hiện ở lane 2 để khớp với vị trí hầm
                for (int i = 0; i < 3; i++)
                {
                    SpawnCoin(2, zLocal + (i * 2f));
                }
                break;

            case 3: // "Bẫy" - Rào thấp chặn cả 3 lane nhưng có 1 cái là hầm chui
                SpawnBarrier(0, zLocal, false);
                SpawnBarrier(1, zLocal, true); // Chỉ chui được ở giữa
                SpawnBarrier(2, zLocal, false);
                break;
        }
    }
    void SpawnBarrier(int lane, float zLocal, bool isHigh)
    {
        GameObject prefab = isHigh ? barrierHighPrefab : barrierLowPrefab;

        // Quyết định chiều cao dựa trên loại rào
        float yPos = isHigh ? barrierHighHeight : barrierLowHeight;

        Vector3 localPos = new Vector3(GetLaneX(lane), yPos, zLocal);
        Vector3 worldPos = transform.TransformPoint(localPos);

        Instantiate(prefab, worldPos, transform.rotation, transform);
    }

    // Hàm bổ trợ Spawn Tunnel
    void SpawnTunnel(float zLocal)
    {
        if (tunnelPrefab == null) return;

        // Sử dụng centerX làm gốc, cộng thêm độ lệch bạn yêu cầu
        // X = 2.14, Y = 0.52
        Vector3 localPos = new Vector3(centerX + 0.34f, 0.52f, zLocal);

        Vector3 worldPos = transform.TransformPoint(localPos);

        Instantiate(tunnelPrefab, worldPos, transform.rotation, transform);
    }

    void SpawnTrainPattern(float zLocal)
    {
        int pattern = Random.Range(0, 3);
        switch (pattern)
        {
            case 0:
                SpawnTrain(0, zLocal); SpawnTrain(2, zLocal); break;
            case 1:
                SpawnTrain(0, zLocal); SpawnTrain(1, zLocal); break;
            case 2:
                SpawnTrain(1, zLocal); SpawnTrain(2, zLocal); break;
        }
    }

    void SpawnCoinPattern(float zLocal)
    {
        int pattern = Random.Range(0, 4); // Thêm pattern mới
        int lane = Random.Range(0, 3);

        switch (pattern)
        {
            case 0: // Đường thẳng
                for (int i = 0; i < 5; i++)
                    SpawnCoin(lane, zLocal + (i * 1.5f));
                break;
            case 1: // Zigzag
                for (int i = 0; i < 3; i++)
                    SpawnCoin(i, zLocal + (i * 2f));
                break;
            case 2: // Vòng cung nhảy (Parabola Arc)
                int arcLength = 7; // Số lượng xu trong 1 vòng cung (tăng lên cho dài)
                float horizontalSpacing = 2.0f; // Khoảng cách giữa các đồng xu theo trục Z
                float maxArcHeight = 1.4f; // ĐÂY LÀ CHIỀU CAO NHẢY CỦA PLAYER (Hãy chỉnh số này)

                for (int i = 0; i < arcLength; i++)
                {

                    float t = (float)i / (arcLength - 1);
                    float parabolaHeight = 4 * maxArcHeight * t * (1 - t);

                    float finalHeight = coinHeight + parabolaHeight;
                    float zPos = zLocal + (i * horizontalSpacing);

                    SpawnCoin(lane, zPos, finalHeight);
                }
                break;
            case 3: // Bậc thang đổi lane
                for (int i = 0; i < 3; i++)
                {
                    SpawnCoin(0, zLocal + (i * 1.5f));
                    SpawnCoin(1, zLocal + (i * 1.5f) + 1.5f);
                    SpawnCoin(2, zLocal + (i * 1.5f) + 3f);
                }
                break;
        }
    }

    float GetLaneX(int lane) => centerX + (lane - 1) * laneDistance;

    void SpawnTrain(int lane, float zLocal)
    {
        Vector3 localPos = new Vector3(GetLaneX(lane), trainHeight, zLocal);
        Vector3 worldPos = transform.TransformPoint(localPos);
        GameObject train = Instantiate(trainPrefab, worldPos, transform.rotation, transform);

        // Tỷ lệ 30% tàu sẽ chạy ngược lại
        if (Random.value < 0.3f)
        {
            MovingTrain mover = train.AddComponent<MovingTrain>();
            mover.speed = Random.Range(15f, 30f); // Tốc độ ngẫu nhiên
        }
    }

    void SpawnCoin(int lane, float zLocal, float customHeight = -1f)
    {
        float finalHeight = (customHeight == -1f) ? coinHeight : customHeight;
        Vector3 localPos = new Vector3(GetLaneX(lane), finalHeight, zLocal);
        Vector3 worldPos = transform.TransformPoint(localPos);

        Instantiate(coinPrefab, worldPos, transform.rotation, transform);
    }
}