using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameSceneName = "subway";
    [SerializeField] private string menuSceneName = "Menu";

    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject deathMenuPanel;

    [Header("Currency UI (Main Screen)")]
    public TextMeshProUGUI txtTotalCoinsDisplay;
    public TextMeshProUGUI txtTotalDiamondsDisplay;

    [Header("Death Menu UI")]
    public TextMeshProUGUI txtRunScore;
    public TextMeshProUGUI txtHighScore;
    public TextMeshProUGUI txtRunCoin;
    public GameObject btnDoubleUp;

    [Header("Audio Settings UI")]
    public Slider musicSlider;       // Thanh kéo to nhỏ
    public Image musicIconDisplay;   // Ảnh nút nhạc
    public Sprite musicOnSprite;     // Hình nốt nhạc đẹp
    public Sprite musicOffSprite;    // Hình nốt nhạc tắt

    // Nếu Hải muốn làm cả nút Sound (Tiếng động FX) thì giữ lại, không thì xóa
    public Image soundIconDisplay;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Header("Transition")]
    public Animator cloudAnimator;

    // --- MỚI THÊM: HIỂN THỊ NHÂN VẬT Ở MENU ---
    [Header("Character Display")]
    public Transform characterSpawnPoint;    // Kéo cái vị trí đứng vào đây
    public GameObject[] allCharacterPrefabs; // Kéo danh sách các Prefab nhân vật vào đây
    private GameObject currentCharacterModel;

    private int coinsCollectedInRun;

    void Start()
    {
        // 1. Cập nhật Tiền
        UpdateTotalCurrencyUI();

        // 2. Cập nhật UI Âm thanh theo trạng thái của MusicManager
        if (MusicManager.Instance != null)
        {
            if (musicSlider != null)
            {
                musicSlider.value = MusicManager.Instance.musicSource.volume;
                musicSlider.onValueChanged.AddListener(SetMusicVolume);
            }
            UpdateMusicIconUI();
        }

        // 3. --- MỚI THÊM: LOAD NHÂN VẬT ĐANG CHỌN ---
        LoadSelectedCharacter();
    }

    void Update()
    {
        // Các phím tắt test game
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) PlayGame();
        if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();

        // Cheat tiền test shop
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerDataManager.Instance.AddDiamonds(100);
            UpdateTotalCurrencyUI();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Test Death Menu!");
            // Gọi hàm hiện bảng với số liệu giả (Điểm 1234, Tiền 555)
            ShowDeathMenu(1234, 555);
        }
    }

    // --- HÀM MỚI: TÌM VÀ HIỂN THỊ NHÂN VẬT ---
    public void LoadSelectedCharacter()
    {
        // Lấy tên nhân vật đang được chọn trong Data
        string selectedName = PlayerDataManager.Instance.GetSelectedCharacter();

        // Tìm xem có Prefab nào trùng tên không
        GameObject prefabToSpawn = null;
        foreach (var prefab in allCharacterPrefabs)
        {
            // Quan trọng: Tên Prefab phải giống hệt tên trong Data (Tricky, Dino...)
            if (prefab.name == selectedName)
            {
                prefabToSpawn = prefab;
                break;
            }
        }

        // Nếu tìm thấy thì tạo ra
        if (prefabToSpawn != null && characterSpawnPoint != null)
        {
            // Xóa con cũ đi (nếu có)
            if (currentCharacterModel != null) Destroy(currentCharacterModel);

            // Tạo con mới tại vị trí SpawnPoint
            currentCharacterModel = Instantiate(prefabToSpawn, characterSpawnPoint.position, characterSpawnPoint.rotation);

            // Gán làm con của SpawnPoint cho gọn gàng
            currentCharacterModel.transform.SetParent(characterSpawnPoint);
        }
    }
    // ------------------------------------------

    // --- CÁC HÀM XỬ LÝ ÂM THANH (GỌI SANG MUSIC MANAGER) ---
    public void OnMusicToggleClick()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.ToggleMusic();
            UpdateMusicIconUI();
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(volume);
        }
    }

    private void UpdateMusicIconUI()
    {
        if (musicIconDisplay != null && MusicManager.Instance != null)
        {
            musicIconDisplay.sprite = MusicManager.Instance.IsMusicOn() ? musicOnSprite : musicOffSprite;
        }
    }

    // --- CÁC HÀM LOGIC GAME KHÁC ---
    public void ShowDeathMenu(int score, int coins)
    {
        coinsCollectedInRun = coins;
        PlayerDataManager.Instance.CheckAndSaveHighScore(score);
        PlayerDataManager.Instance.AddCoins(coinsCollectedInRun);

        deathMenuPanel.SetActive(true);
        if (btnDoubleUp != null) btnDoubleUp.SetActive(true);

        txtRunScore.text = score.ToString();
        txtRunCoin.text = coinsCollectedInRun.ToString();
        txtHighScore.text = PlayerDataManager.Instance.GetHighScore().ToString();

        UpdateTotalCurrencyUI();
        Time.timeScale = 0f;
    }

    public void WatchAdDoubleCoin()
    {
        StartCoroutine(SimulateAdRoutine());
    }

    IEnumerator SimulateAdRoutine()
    {
        Debug.Log("Đang xem quảng cáo...");
        yield return new WaitForSecondsRealtime(2.0f);
        PlayerDataManager.Instance.AddCoins(coinsCollectedInRun);
        txtRunCoin.text = (coinsCollectedInRun * 2).ToString();
        UpdateTotalCurrencyUI();
        if (btnDoubleUp != null) btnDoubleUp.SetActive(false);
    }

    public void UpdateTotalCurrencyUI()
    {
        if (txtTotalCoinsDisplay != null)
            txtTotalCoinsDisplay.text = PlayerDataManager.Instance.GetTotalCoins().ToString("N0");
        if (txtTotalDiamondsDisplay != null)
            txtTotalDiamondsDisplay.text = PlayerDataManager.Instance.GetTotalDiamonds().ToString("N0");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoBackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    public void OpenSettings() { settingsPanel.SetActive(true); }
    public void CloseSettings() { settingsPanel.SetActive(false); }
    public void OpenShop() { SceneManager.LoadScene("Shop"); }
    public void CloseShop() { if (shopPanel != null) shopPanel.SetActive(false); }

    public void PlayGame()
    {
        if (cloudAnimator != null && cloudAnimator.gameObject.activeInHierarchy) cloudAnimator.Play("Cloud_Open");
        Invoke("LoadSubwayScene", 1.0f);
    }
    void LoadSubwayScene() { SceneManager.LoadScene(gameSceneName); }
    public void QuitGame() { Application.Quit(); }
}