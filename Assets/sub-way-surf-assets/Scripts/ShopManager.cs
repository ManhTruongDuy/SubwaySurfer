using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Data & Transform")]
    public CharacterShopData[] allCharacters; // Danh sách Data (Hải mới có 1 con thì kéo 1 con vào)
    public Transform spawnPoint;              // Điểm đứng trên bục

    [Header("UI Reference")]
    public TextMeshProUGUI btnText;           // Chữ trên nút (BUY / SELECT / SELECTED)
    public TextMeshProUGUI priceText;         // Chữ giá tiền
    public Button buyButton;                  // Cái nút bấm để đổi màu nếu cần

    private int currentIndex = 0;
    private GameObject currentModel;

    void Start()
    {
        // Khi mới vào Shop, luôn hiện nhân vật đang được chọn (Selected)
        // Tìm xem nhân vật nào đang Selected để hiện nó đầu tiên
        string selectedName = PlayerDataManager.Instance.GetSelectedCharacter();
        int indexToShow = 0;

        for (int i = 0; i < allCharacters.Length; i++)
        {
            if (allCharacters[i].characterName == selectedName)
            {
                indexToShow = i;
                break;
            }
        }

        UpdateShopUI(indexToShow);
    }

    public void UpdateShopUI(int index)
    {
        // Chặn lỗi: Nếu Hải chỉ có 1 data mà lỡ bấm nút số 2 -> Return ngay để không lỗi đỏ
        if (index >= allCharacters.Length || index < 0) return;

        currentIndex = index;
        CharacterShopData data = allCharacters[index];

        // 1. Xử lý Model 3D (Xóa cũ tạo mới)
        if (currentModel != null) Destroy(currentModel);
        currentModel = Instantiate(data.modelPrefab, spawnPoint.position, spawnPoint.rotation);
        currentModel.transform.SetParent(spawnPoint);

        // Ép nhân vật nhảy về chính giữa điểm Spawn (tọa độ 0,0,0 so với cha)
        currentModel.transform.localPosition = Vector3.zero;
        currentModel.transform.localRotation = Quaternion.identity; // Reset góc xoay luôn cho thẳng
        

        // 2. --- LOGIC 3 TRẠNG THÁI QUAN TRỌNG ---
        bool isUnlocked = PlayerDataManager.Instance.IsCharacterUnlocked(data.characterName);
        string currentSelectedChar = PlayerDataManager.Instance.GetSelectedCharacter();

        if (isUnlocked)
        {
            // Trạng thái: ĐÃ MUA
            priceText.text = "OWNED";

            if (currentSelectedChar == data.characterName)
            {
                // Trạng thái 1: ĐANG SỬ DỤNG (SELECTED)
                btnText.text = "SELECTED";
                buyButton.interactable = false; // Làm mờ nút đi để người chơi biết là đang dùng rồi
            }
            else
            {
                // Trạng thái 2: ĐÃ MUA NHƯNG CHƯA DÙNG (SELECT)
                btnText.text = "SELECT";
                buyButton.interactable = true; // Cho phép bấm để chọn
            }
        }
        else
        {
            // Trạng thái 3: CHƯA MUA (BUY)
            priceText.text = data.price.ToString("N0"); // Hiện giá tiền
            btnText.text = "BUY";
            buyButton.interactable = true;
        }
    }

    public void OnClickBuyOrEquip()
    {
        CharacterShopData data = allCharacters[currentIndex];
        bool isUnlocked = PlayerDataManager.Instance.IsCharacterUnlocked(data.characterName);

        if (isUnlocked)
        {
            // Nếu đã mua rồi -> Bấm nút nghĩa là CHỌN (Select)
            PlayerDataManager.Instance.SetSelectedCharacter(data.characterName);
            Debug.Log("Đã chọn nhân vật: " + data.characterName);
        }
        else
        {
            // Nếu chưa mua -> Bấm nút nghĩa là MUA (Buy)
            if (PlayerDataManager.Instance.SpendCoins(data.price))
            {
                PlayerDataManager.Instance.UnlockCharacter(data.characterName);
                PlayerDataManager.Instance.SetSelectedCharacter(data.characterName); // Mua xong tự chọn luôn
                Debug.Log("Mua thành công!");
            }
            else
            {
                Debug.Log("Không đủ tiền!");
                // Chỗ này Hải có thể làm cái bảng thông báo "Nạp thêm tiền đê"
                return; // Thoát ra không cập nhật UI
            }
        }

        // Cập nhật lại giao diện ngay lập tức sau khi bấm
        UpdateShopUI(currentIndex);
    }

    public void BackToMenu()
    {
        // Quay về Scene tên là "Menu" (hoặc tên Scene menu của Hải)
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}