using UnityEngine;

public class SpawnPatternSystem : MonoBehaviour
{
    public GameObject trainPrefab;
    public GameObject coinPrefab;

    [Header("Settings")]
    public float centerX = 0f;// là 0 nếu model đã căn giữa, hoặc điều chỉnh lại
    public float laneDistance = 2.04f;
    public float trainHeight = -0.35f;
    public float coinHeight = 0.75f;

    [Header("Spacing")]
    public float segmentLength = 30f;
    public float sectionStep = 10f;



    public void GenerateSegmentContent(float length)
    {
        segmentLength = length;
        // Ta dùng zLocal để tính khoảng cách nội bộ trong 1 segment
        for (float zLocal = 5f; zLocal < segmentLength; zLocal += sectionStep)
        {
            int roll = Random.Range(0, 100);

            if (roll < 40)
                SpawnTrainPattern(zLocal);
            else if (roll < 80)
                SpawnCoinPattern(zLocal);
        }
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