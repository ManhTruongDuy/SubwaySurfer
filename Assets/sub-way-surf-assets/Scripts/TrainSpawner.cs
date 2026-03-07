using UnityEngine;

public class TrainSpawner : MonoBehaviour
{
    public GameObject trainPrefab;
    public Transform player;

    public float laneDistance = 3f;
    public float spawnDistance = 40f;

    void Start()
    {
        float centerX = player.position.x;

        int lane = Random.Range(0, 3);

        float laneX = centerX + (lane - 1) * laneDistance;

        Vector3 pos = new Vector3(
            laneX,
            0,
            player.position.z + spawnDistance
        );

        Instantiate(trainPrefab, pos, Quaternion.identity);
    }
}