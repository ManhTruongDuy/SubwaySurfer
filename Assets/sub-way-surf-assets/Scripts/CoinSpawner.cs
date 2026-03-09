using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;

    public float centerX = -12.67f;
    public float laneDistance = 3f;

    public int coinRows = 10;
    public float coinSpacing = 2f;
    public float coinHeight = 1.5f;

    void Start()
    {
        SpawnCoins();
    }

    void SpawnCoins()
    {
        int lane = Random.Range(0, 3);

        float laneX = centerX + (lane - 1) * laneDistance;

        float startZ = transform.position.z + 5f;

        for (int i = 0; i < coinRows; i++)
        {
            Vector3 pos = new Vector3(
                laneX,
                coinHeight,
                startZ + i * coinSpacing
            );

            Instantiate(coinPrefab, pos, Quaternion.identity, transform);
        }
    }
}