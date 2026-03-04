using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance; // Singleton để gọi từ bất cứ đâu

    private void Awake()
    {
        // Đảm bảo chỉ có 1 PlayerDataManager tồn tại trong game
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Không bị hủy khi chuyển Scene
            if (!IsCharacterUnlocked("Tricky"))
            {
                UnlockCharacter("Tricky");
                SetSelectedCharacter("Tricky"); // Mặc định chọn luôn
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- XỬ LÝ VÀNG (COINS) ---
    public int GetTotalCoins()
    {
        return PlayerPrefs.GetInt("TotalCoins", 0);
    }

    public void AddCoins(int amount)
    {
        int current = GetTotalCoins();
        PlayerPrefs.SetInt("TotalCoins", current + amount);
        PlayerPrefs.Save();
    }

    public bool SpendCoins(int amount)
    {
        int current = GetTotalCoins();
        if (current >= amount)
        {
            PlayerPrefs.SetInt("TotalCoins", current - amount);
            PlayerPrefs.Save();
            return true; // Mua thành công
        }
        return false; // Không đủ tiền
    }

    // --- XỬ LÝ KIM CƯƠNG (DIAMONDS) ---
    public int GetTotalDiamonds()
    {
        return PlayerPrefs.GetInt("TotalDiamonds", 0);
    }

    public void AddDiamonds(int amount)
    {
        int current = GetTotalDiamonds();
        PlayerPrefs.SetInt("TotalDiamonds", current + amount);
        PlayerPrefs.Save();
    }

    public bool SpendDiamonds(int amount)
    {
        int current = GetTotalDiamonds();
        if (current >= amount)
        {
            PlayerPrefs.SetInt("TotalDiamonds", current - amount);
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    // --- XỬ LÝ ĐIỂM CAO (HIGH SCORE) ---
    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    public void CheckAndSaveHighScore(int newScore)
    {
        int currentHigh = GetHighScore();
        if (newScore > currentHigh)
        {
            PlayerPrefs.SetInt("HighScore", newScore);
            PlayerPrefs.Save();
        }
    }

    // Kiểm tra xem nhân vật đã mua chưa
    public bool IsCharacterUnlocked(string name)
    {
        // Nhân vật mặc định (Tricky) luôn là 1 (đã mở)
        if (name == "Tricky") return true;
        return PlayerPrefs.GetInt("Unlock_" + name, 0) == 1;
    }

    // Đánh dấu đã mua nhân vật
    public void UnlockCharacter(string name)
    {
        PlayerPrefs.SetInt("Unlock_" + name, 1);
        PlayerPrefs.Save();
    }

    // --- XỬ LÝ NHÂN VẬT ĐANG CHỌN ĐỂ CHƠI ---
    public string GetSelectedCharacter()
    {
        return PlayerPrefs.GetString("SelectedCharacter", "Tricky");
    }

    public void SetSelectedCharacter(string name)
    {
        PlayerPrefs.SetString("SelectedCharacter", name);
        PlayerPrefs.Save();
    }
}