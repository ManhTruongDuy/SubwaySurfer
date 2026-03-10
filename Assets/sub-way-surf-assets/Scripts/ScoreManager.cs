using UnityEngine;
using TMPro; // Bắt buộc phải có dòng này để dùng TextMeshPro

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance; // Để các script khác (như Character) gọi được

    [Header("Data")]
    public int score; // Điểm (Quãng đường)
    public int coins; // Vàng (Hải đã làm xong)

    [Header("Distance Tracking")]
    public Transform player;  // Kéo PlayerRoot vào đây để theo dõi vị trí
    private float startZ;     // Lưu lại tọa độ Z lúc xuất phát

    [Header("UI References")]
    public TextMeshProUGUI scoreText; // Chữ hiển thị điểm lúc đang chạy
    public TextMeshProUGUI coinText;  // Chữ hiển thị vàng lúc đang chạy

    void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        // Ghi nhớ tọa độ Z lúc mới vào game
        if (player != null)
        {
            startZ = player.position.z;
        }
    }

    void Update()
    {
        // Tính quãng đường = Vị trí Z hiện tại trừ đi Vị trí Z lúc xuất phát
        if (player != null)
        {
            // Dùng Mathf.FloorToInt để làm tròn số (chỉ lấy số nguyên)
            score = Mathf.FloorToInt(player.position.z - startZ);

            // Cập nhật số lên màn hình UI đang chơi
            if (scoreText != null)
            {
                scoreText.text = "Score: " + score.ToString();
            }
        }
    }

    // (Hàm ăn vàng của Hải chắc đang nằm ở đây, cứ giữ nguyên nhé)
    public void AddCoin(int amount)
    {
        coins += amount; // Cộng thêm đúng số lượng vàng mà đồng xu gửi tới
        if (coinText != null) coinText.text = "Coins: " + coins.ToString();
    }
}