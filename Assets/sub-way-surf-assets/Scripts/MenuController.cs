using UnityEngine;
using UnityEngine.SceneManagement; // Thư viện để quản lý Scene

public class MenuController : MonoBehaviour
{
    // Tên của Scene game chính (phải khớp chính xác với tên file scene)
    [SerializeField] private string gameSceneName = "subway";

    void Update()
    {
        // Xử lý Input cho PC: Phím Space hoặc Enter để vào game nhanh
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            PlayGame();
        }

        // Phím ESC để thoát game (tùy chọn thêm)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    // Hàm gọi khi nhấn nút Play hoặc phím Space/Enter
    public void PlayGame()
    {
        Debug.Log("Loading Scene: " + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    // Hàm gọi khi nhấn nút Quit
    public void QuitGame()
    {
        Debug.Log("Đã thoát game (Lệnh này chỉ hoạt động khi Build ra file .exe)");
        Application.Quit();
    }
}