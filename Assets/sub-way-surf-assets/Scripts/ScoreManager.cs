using UnityEngine;
using TMPro; // Bắt buộc phải có dòng này để dùng TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; 

    [Header("Data")]
    public int score; 
    public int coins;

    [Header("Distance Tracking")]
    public Transform player;  
    private float startZ;     

    [Header("UI References")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI coinText;  

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        
        if (player != null)
        {
            startZ = player.position.z;
        }
    }

    void Update()
    {
       
        if (player != null)
        {
            
            score = Mathf.FloorToInt(player.position.z - startZ);

            
            if (scoreText != null)
            {
                scoreText.text = "Score: " + score.ToString();
            }
        }
    }

    
    public void AddCoin(int amount)
    {
        coins += amount; 
        if (coinText != null) coinText.text = "Coins: " + coins.ToString();
    }
}