using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource musicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Tự động load âm lượng cũ
            float savedVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSource.volume = savedVol;

            // Tự động load trạng thái tắt/bật
            int isMuted = PlayerPrefs.GetInt("MusicMute", 0);
            musicSource.mute = (isMuted == 1);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
        PlayerPrefs.SetInt("MusicMute", musicSource.mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    // --- HÀM MỚI ĐỂ CHỈNH THANH TRƯỢT ---
    public void SetVolume(float vol)
    {
        musicSource.volume = vol;
        PlayerPrefs.SetFloat("MusicVolume", vol);
        PlayerPrefs.Save();
    }

    public bool IsMusicOn()
    {
        return !musicSource.mute;
    }
}