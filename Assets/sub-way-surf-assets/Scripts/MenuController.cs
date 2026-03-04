using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "subway";
    [SerializeField] private GameObject settingsPanel;

    [Header("Music Settings")]
    public AudioSource bgMusicSource;
    public Slider musicSlider;
    public Image musicIconDisplay;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    [Header("Sound Settings")]
    public Image soundIconDisplay;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Header("Transition")]
    public Animator cloudAnimator;

    [SerializeField] private GameObject shopPanel;

    [Header("Death Menu UI")]
    public GameObject deathMenuPanel;
    public TextMeshProUGUI txtCurrentScore;
    public TextMeshProUGUI txtHighScore;
    public TextMeshProUGUI txtCoin;
    public GameObject btnDoubleUp; // Kéo nút Watch Video vào đây để ẩn sau khi xem

    private int currentCoins; // Biến lưu số coin tạm thời để x2

    void Start()
    {
        if (bgMusicSource != null && musicSlider != null)
        {
            musicSlider.value = bgMusicSource.volume;
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            PlayGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        // --- TEST NHANH ---
        // Nhấn phím K để giả lập nhân vật chết ngay lập tức
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Truyền thử 1234 điểm và 50 coin để test
            ShowDeathMenu(1234, 50); 
        }
    }

    public void ShowDeathMenu(int score, int coins)
    {
        currentCoins = coins; // Lưu lại số coin ván này
        deathMenuPanel.SetActive(true);
        if (btnDoubleUp != null) btnDoubleUp.SetActive(true); // Reset nút x2 hiện lên

        txtCurrentScore.text = score.ToString();
        txtCoin.text = currentCoins.ToString();

        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
        if (score > savedHighScore)
        {
            savedHighScore = score;
            PlayerPrefs.SetInt("HighScore", savedHighScore);
        }
        txtHighScore.text = savedHighScore.ToString();

        Time.timeScale = 0f; // Dừng game
    }

    // --- TÍNH NĂNG MỚI: X2 COIN GIẢ LẬP ---
    public void WatchAdDoubleCoin()
    {
        StartCoroutine(SimulateAdRoutine());
    }

    IEnumerator SimulateAdRoutine()
    {
        Debug.Log("Đang xem quảng cáo...");
        // Dùng WaitForSecondsRealtime vì Time.timeScale đang bằng 0
        yield return new WaitForSecondsRealtime(2.0f);

        currentCoins *= 2; // Nhân đôi số tiền
        txtCoin.text = currentCoins.ToString();

        if (btnDoubleUp != null) btnDoubleUp.SetActive(false); // Xem xong thì ẩn nút
        Debug.Log("Đã nhân đôi Coin!");
    }

    // --- TÍNH NĂNG MỚI: CHƠI LẠI (RESTART) ---
    public void RestartGame()
    {
        Time.timeScale = 1f; // Trả lại thời gian bình thường
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Load lại chính cảnh hiện tại
    }

    public void OpenSettings() { settingsPanel.SetActive(true); }
    public void CloseSettings() { settingsPanel.SetActive(false); }

    public void OnSoundToggle(bool isOn)
    {
        if (soundIconDisplay != null)
            soundIconDisplay.sprite = isOn ? soundOnSprite : soundOffSprite;
    }

    public void SetMusicVolume(float volume)
    {
        if (bgMusicSource != null) bgMusicSource.volume = volume;
    }

    public void OnMusicToggle(bool isOn)
    {
        if (musicIconDisplay != null)
            musicIconDisplay.sprite = isOn ? musicOnSprite : musicOffSprite;
        if (bgMusicSource != null) bgMusicSource.mute = !isOn;
    }

    public void PlayGame()
    {
        if (cloudAnimator != null) cloudAnimator.Play("Cloud_Open");
        Invoke("LoadSubwayScene", 1.0f);
    }

    void LoadSubwayScene() { SceneManager.LoadScene(gameSceneName); }

    public void OpenShop() { if (shopPanel != null) shopPanel.SetActive(true); }
    public void CloseShop() { if (shopPanel != null) shopPanel.SetActive(false); }

    public void GoBackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Đã thoát game");
        Application.Quit();
    }
}