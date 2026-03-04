using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "subway";
    [SerializeField] private GameObject settingsPanel;

    [Header("Music Settings")] // Phân nhóm cho dễ nhìn trong Inspector
    public AudioSource bgMusicSource; // Nguồn nhạc nền
    public Slider musicSlider;        // Thanh kéo âm lượng
    public Image musicIconDisplay;
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;

    [Header("Sound Settings")] // Thêm các biến mới cho Sound
    public Image soundIconDisplay;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;

    void Start()
    {
        // Khi bắt đầu, đặt giá trị Slider khớp với âm lượng hiện tại
        if (bgMusicSource != null && musicSlider != null)
        {
            musicSlider.value = bgMusicSource.volume;
            // Lắng nghe sự kiện kéo thanh trượt
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
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

 

    // Hàm điều khiển Icon Sound (Hàm mới)
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

        if (bgMusicSource != null) bgMusicSource.mute = !isOn; // Tắt tiếng khi icon là Off
    }
    public void PlayGame()
    {
        Debug.Log("Loading Scene: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Đã thoát game");
        Application.Quit();
    }
}