using UnityEngine;

public class LaneSpawnSystem : MonoBehaviour
{
    public GameObject trainPrefab;
    public GameObject coinPrefab;

    public float centerX = -12.67f;
    public float laneDistance = 3f;

    public int coinRows = 10;
    public float coinSpacing = 2f;

    public int maxTrainChain = 3;
    public float trainSpacing = 10f;

    void Start()
    {
        SpawnSegmentContent();
    }

    void SpawnSegmentContent()
    {
        int safeLane = Random.Range(0, 3);

        float startZ = transform.position.z + 10;

        for (int lane = 0; lane < 3; lane++)
        {
            float laneX = centerX + (lane - 1) * laneDistance;

            if (lane == safeLane)
            {
                SpawnCoins(laneX, startZ);
            }
            else
            {
                SpawnTrainChain(laneX, startZ);
            }
        }
    }

    void SpawnCoins(float laneX, float startZ)
    {
        for (int i = 0; i < coinRows; i++)
        {
            Vector3 pos = new Vector3(
                laneX,
                1.5f,
                startZ + i * coinSpacing
            );

            Instantiate(coinPrefab, pos, Quaternion.identity, transform);
        }
    }

    void SpawnTrainChain(float laneX, float startZ)
    {
        int chain = Random.Range(1, maxTrainChain + 1);

        for (int i = 0; i < chain; i++)
        {
            Vector3 pos = new Vector3(
                laneX,
                0,
                startZ + i * trainSpacing
            );

            Instantiate(trainPrefab, pos, Quaternion.identity, transform);
        }
    }
}