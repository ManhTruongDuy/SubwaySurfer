using UnityEngine;

public class TrainSpawner : MonoBehaviour
{
    public GameObject trainPrefab;
    public GameObject coinPrefab;

    public float centerX = -12.67f;
    public float laneDistance = 3f;

    public int coinRows = 10;
    public float coinSpacing = 2f;

    public int maxTrainChain = 3;

    void Start()
    {
        SpawnPattern();
    }

    void SpawnPattern()
    {
        int safeLane = Random.Range(0, 3);

        for (int lane = 0; lane < 3; lane++)
        {
            float laneX = centerX + (lane - 1) * laneDistance;

            if (lane != safeLane)
            {
                int chain = Random.Range(1, maxTrainChain + 1);

                for (int i = 0; i < chain; i++)
                {
                    Vector3 pos = new Vector3(
                        laneX,
                        0,
                        transform.position.z + 12 + i * 10
                    );

                    Instantiate(trainPrefab, pos, Quaternion.identity, transform);
                }
            }
            else
            {
                for (int i = 0; i < coinRows; i++)
                {
                    Vector3 pos = new Vector3(
                        laneX,
                        1.5f,
                        transform.position.z + 5 + i * coinSpacing
                    );

                    Instantiate(coinPrefab, pos, Quaternion.identity, transform);
                }
            }
        }
    }
}