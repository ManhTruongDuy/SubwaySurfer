using UnityEngine;

public class CoinRotate : MonoBehaviour
{
    public float rotateSpeed = 150f;

    void Update()
    {
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}