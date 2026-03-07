using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    public GameObject coinPrefab;

    public Transform player;

    public float laneDistance = 3f;
    public int coinRows = 10;
    public float coinSpacing = 2f;
    public float coinHeight = 1.5f;

    void Start()
    {
        float centerX = player.position.x;

        int lane = Random.Range(0, 3);

        float laneX = centerX + (lane - 1) * laneDistance;

        for (int i = 0; i < coinRows; i++)
        {
            Vector3 pos = new Vector3(
                laneX,
                coinHeight,
                transform.position.z + 5 + i * coinSpacing
            );

            Instantiate(coinPrefab, pos, Quaternion.identity, transform);
        }
    }
}