using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject deathMenuPanel;

    [Header("Death Menu UI")]
    public TextMeshProUGUI txtRunScore;
    public TextMeshProUGUI txtHighScore;
    public TextMeshProUGUI txtRunCoin;
    public GameObject btnDoubleUp;

    private int coinsCollectedInRun;

    public void ShowDeathMenu(int score, int coins)
    {
        coinsCollectedInRun = coins;

        if (PlayerDataManager.Instance == null)
        {
            Debug.LogError("🚨 BÁO ĐỘNG: Không tìm thấy PlayerDataManager trên màn hình này!");
        }

        // Lưu điểm cao và cộng tiền (Cần đảm bảo PlayerDataManager tồn tại)
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.CheckAndSaveHighScore(score);
            PlayerDataManager.Instance.AddCoins(coinsCollectedInRun);
            if (txtHighScore != null)
            {
                txtHighScore.text = PlayerDataManager.Instance.GetHighScore().ToString();
            }
        }

        if (deathMenuPanel != null) deathMenuPanel.SetActive(true);
        if (btnDoubleUp != null) btnDoubleUp.SetActive(true);

        if (txtRunScore != null) txtRunScore.text = score.ToString();
        if (txtRunCoin != null) txtRunCoin.text = coinsCollectedInRun.ToString();

        Time.timeScale = 0f;
    }

    public void WatchAdDoubleCoin()
    {
        StartCoroutine(SimulateAdRoutine());
    }

    IEnumerator SimulateAdRoutine()
    {
        Debug.Log("Đang xem quảng cáo...");
        // Dừng thời gian thật (Realtime) để chờ quảng cáo, vì Time.timeScale đang là 0
        yield return new WaitForSecondsRealtime(2.0f);
        
        if (PlayerDataManager.Instance != null)
        {
            PlayerDataManager.Instance.AddCoins(coinsCollectedInRun);
        }
        
        if (txtRunCoin != null) txtRunCoin.text = (coinsCollectedInRun * 2).ToString();
        if (btnDoubleUp != null) btnDoubleUp.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoBackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Thay "Menu" bằng tên scene menu chính của bạn
    }
}
