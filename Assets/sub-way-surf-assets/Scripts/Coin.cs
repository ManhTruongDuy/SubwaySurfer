using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ScoreManager.instance.AddCoin(value);
            Destroy(gameObject);
        }
    }
}