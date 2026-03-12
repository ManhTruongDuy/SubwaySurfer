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
    public Slider musicSlider;       
    public Image musicIconDisplay;   
    public Sprite musicOnSprite;     
    public Sprite musicOffSprite;    

    
    public Image soundIconDisplay;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    [Header("Transition")]
    public Animator cloudAnimator;

    
    [Header("Character Display")]
    public Transform characterSpawnPoint;
    public GameObject[] allCharacterPrefabs;
    private GameObject currentCharacterModel;

    private int coinsCollectedInRun;

    void Start()
    {
        UpdateTotalCurrencyUI();
        if (MusicManager.Instance != null)
        {
            if (musicSlider != null)
            {
                musicSlider.value = MusicManager.Instance.musicSource.volume;
                musicSlider.onValueChanged.AddListener(SetMusicVolume);
            }
            UpdateMusicIconUI();
        }

        LoadSelectedCharacter();
    }

    void Update()
    {
        // Các phím tắt test game
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) PlayGame();
        if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();

        // Cheat tiền test
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerDataManager.Instance.AddDiamonds(100);
            UpdateTotalCurrencyUI();
        }

    }

    
    public void LoadSelectedCharacter()
    {
        string selectedName = PlayerDataManager.Instance.GetSelectedCharacter();

        GameObject prefabToSpawn = null;
        foreach (var prefab in allCharacterPrefabs)
        {
            //Tên Prefab phải giống hệt tên trong Data
            if (prefab.name == selectedName)
            {
                prefabToSpawn = prefab;
                break;
            }
        }

        if (prefabToSpawn != null && characterSpawnPoint != null)
        {
            if (currentCharacterModel != null) Destroy(currentCharacterModel);

            currentCharacterModel = Instantiate(prefabToSpawn, characterSpawnPoint.position, characterSpawnPoint.rotation);

            currentCharacterModel.transform.SetParent(characterSpawnPoint);
        }
    }
   
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