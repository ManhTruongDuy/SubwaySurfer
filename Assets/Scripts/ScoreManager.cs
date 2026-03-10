using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score;
    public int coins;

    void Awake()
    {
        instance = this;
    }

    public void AddCoin(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }

    public void AddScore(int amount)
    {
        score += amount;
    }
}