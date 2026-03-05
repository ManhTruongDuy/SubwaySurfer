using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    [Header("Data")]
    public CharacterShopData[] allCharacters;
    public Transform spawnPoint;

    [Header("UI Tabs (Chuyển trang)")]
    public GameObject characterShopPanel; // Kéo cái Panel chứa nút Next/Prev/Buy vào đây
    public GameObject diamondShopPanel;   // Kéo cái trang Shop nạp tiền vào đây
    public GameObject character3DModelParent; // Kéo cái SpawnPoint vào đây (để ẩn hiện 3D)

    [Header("UI Buying (Nút mua)")]
    public Button btnBuyGold;         // Kéo nút Mua Vàng
    public TextMeshProUGUI txtGoldPrice; // Chữ giá tiền trên nút Vàng

    public Button btnBuyDiamond;      // Kéo nút Mua Kim Cương
    public TextMeshProUGUI txtDiamondPrice; // Chữ giá tiền trên nút KC

    public Button btnEquip;           // Tạo thêm 1 nút riêng chỉ hiện chữ "EQUIP/SELECTED"
    public TextMeshProUGUI txtEquip;  // Chữ trên nút Equip

    private int currentIndex = 0;
    private GameObject currentModel;

    [Header("UI Global (Góc màn hình)")]
    public TextMeshProUGUI txtTotalCoins;    // Kéo cái số tiền Vàng ở góc vào đây
    public TextMeshProUGUI txtTotalDiamonds; // Kéo cái số Kim cương ở góc vào đây

    void Start()
    {
        // Mặc định hiện Shop Nhân vật
        OpenCharacterTab();
        UpdateCurrencyUI();
    }

    // --- LOGIC CHUYỂN TAB ---
    public void OpenCharacterTab()
    {
        characterShopPanel.SetActive(true);
        diamondShopPanel.SetActive(false);
        character3DModelParent.gameObject.SetActive(true); // Hiện nhân vật 3D
        UpdateShopUI(currentIndex);
    }

    public void OpenDiamondTab()
    {
        characterShopPanel.SetActive(false);
        diamondShopPanel.SetActive(true);
        character3DModelParent.gameObject.SetActive(false); // Ẩn nhân vật 3D đi cho đỡ rối
    }

    // --- LOGIC HIỂN THỊ SHOP ---
    public void UpdateShopUI(int index)
    {
        if (index >= allCharacters.Length || index < 0) return;
        currentIndex = index;
        CharacterShopData data = allCharacters[index];

        // 1. Xử lý Model 3D
        if (currentModel != null) Destroy(currentModel);
        if (character3DModelParent.gameObject.activeSelf) // Chỉ tạo khi đang ở Tab Character
        {
            currentModel = Instantiate(data.modelPrefab, spawnPoint.position, spawnPoint.rotation);
            currentModel.transform.SetParent(spawnPoint);
            currentModel.transform.localPosition = Vector3.zero;
            currentModel.transform.localRotation = Quaternion.identity;
        }

        // 2. Logic Nút Mua
        bool isUnlocked = PlayerDataManager.Instance.IsCharacterUnlocked(data.characterName);
        string currentSelected = PlayerDataManager.Instance.GetSelectedCharacter();

        if (isUnlocked)
        {
            // TRƯỜNG HỢP 1: ĐÃ MUA RỒI
            // -> Phải TẮT hết 2 nút mua đi (Quan trọng!)
            btnBuyGold.gameObject.SetActive(false);     // Tắt nút Vàng
            btnBuyDiamond.gameObject.SetActive(false);  // Tắt nút Kim cương (Cái nút xanh đang bị lỗi của Hải)

            // -> Chỉ BẬT nút Chọn (Equip) lên thôi
            btnEquip.gameObject.SetActive(true);

            // Logic chữ Selected/Equip
            if (currentSelected == data.characterName)
            {
                txtEquip.text = "SELECTED";
                btnEquip.interactable = false; // Đang chọn rồi thì không bấm nữa
            }
            else
            {
                txtEquip.text = "SELECT"; // Hoặc EQUIP
                btnEquip.interactable = true;
            }
        }
        else
        {
            // TRƯỜNG HỢP 2: CHƯA MUA
            // -> BẬT 2 nút mua lên
            btnBuyGold.gameObject.SetActive(true);
            btnBuyDiamond.gameObject.SetActive(true);

            // -> TẮT nút Chọn đi
            btnEquip.gameObject.SetActive(false);

            // Cập nhật giá tiền
            txtGoldPrice.text = data.price.ToString("N0");
            txtDiamondPrice.text = data.diamondPrice.ToString("N0");
        }
    }

    // --- CÁC HÀM BẤM NÚT ---

    public void OnBuyWithGold()
    {
        CharacterShopData data = allCharacters[currentIndex];
        if (PlayerDataManager.Instance.SpendCoins(data.price))
        {
            UnlockAndSelect(data);
            UpdateCurrencyUI();
        }
        else Debug.Log("Thiếu vàng!");
    }

    public void OnBuyWithDiamond()
    {
        CharacterShopData data = allCharacters[currentIndex];
        // Hàm SpendDiamonds Hải cần viết thêm bên PlayerDataManager nhé
        if (PlayerDataManager.Instance.SpendDiamonds(data.diamondPrice))
        {
            UnlockAndSelect(data);
            UpdateCurrencyUI();
        }
        else Debug.Log("Thiếu kim cương!");
    }

    public void OnEquipClick()
    {
        CharacterShopData data = allCharacters[currentIndex];
        PlayerDataManager.Instance.SetSelectedCharacter(data.characterName);
        UpdateShopUI(currentIndex);
    }

    void UnlockAndSelect(CharacterShopData data)
    {
        PlayerDataManager.Instance.UnlockCharacter(data.characterName);
        PlayerDataManager.Instance.SetSelectedCharacter(data.characterName);
        UpdateShopUI(currentIndex);
    }

    // Hàm nạp tiền thật (Giả lập)
    public void BuyDiamondPack(int amount)
    {
        PlayerDataManager.Instance.AddDiamonds(amount);
        // Cập nhật UI tiền tổng ở góc màn hình...
        UpdateCurrencyUI();
        Debug.Log("Đã nạp " + amount + " kim cương!");
    }

    // Hàm này chuyên dùng để vẽ lại số tiền trên góc màn hình
    public void UpdateCurrencyUI()
    {
        if (txtTotalCoins != null)
            txtTotalCoins.text = PlayerDataManager.Instance.GetTotalCoins().ToString("N0");

        if (txtTotalDiamonds != null)
            txtTotalDiamonds.text = PlayerDataManager.Instance.GetTotalDiamonds().ToString("N0");
    }

    // Nút mũi tên PHẢI (Next)
    public void OnNextClick()
    {
        currentIndex++; // Tăng số thứ tự lên
        // Nếu vượt quá số lượng nhân vật thì quay về con đầu tiên
        if (currentIndex >= allCharacters.Length)
        {
            currentIndex = 0;
        }
        UpdateShopUI(currentIndex); // Cập nhật lại hình ảnh và giá tiền
    }

    // Nút mũi tên TRÁI (Previous)
    public void OnPrevClick()
    {
        currentIndex--; // Giảm số thứ tự xuống
        // Nếu nhỏ hơn 0 thì nhảy về con cuối cùng
        if (currentIndex < 0)
        {
            currentIndex = allCharacters.Length - 1;
        }
        UpdateShopUI(currentIndex);
    }

    // Nút Back, Next, Prev giữ nguyên...
    public void BackToMenu() { SceneManager.LoadScene("Menu"); }
}