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
    public GameObject characterShopPanel;
    public GameObject diamondShopPanel;  
    public GameObject character3DModelParent;

    [Header("UI Buying (Nút mua)")]
    public Button btnBuyGold;     
    public TextMeshProUGUI txtGoldPrice;

    public Button btnBuyDiamond;
    public TextMeshProUGUI txtDiamondPrice;

    public Button btnEquip;
    public TextMeshProUGUI txtEquip;

    private int currentIndex = 0;
    private GameObject currentModel;

    [Header("UI Global (Góc màn hình)")]
    public TextMeshProUGUI txtTotalCoins;
    public TextMeshProUGUI txtTotalDiamonds;

    void Start()
    {
        // Mặc định hiện Shop Nhân vật
        OpenCharacterTab();
        UpdateCurrencyUI();
    }

    
    public void OpenCharacterTab()
    {
        characterShopPanel.SetActive(true);
        diamondShopPanel.SetActive(false);
        character3DModelParent.gameObject.SetActive(true);
        UpdateShopUI(currentIndex);
    }

    public void OpenDiamondTab()
    {
        characterShopPanel.SetActive(false);
        diamondShopPanel.SetActive(true);
        character3DModelParent.gameObject.SetActive(false);
    }

    
    public void UpdateShopUI(int index)
    {
        if (index >= allCharacters.Length || index < 0) return;
        currentIndex = index;
        CharacterShopData data = allCharacters[index];

        
        if (currentModel != null) Destroy(currentModel);
        if (character3DModelParent.gameObject.activeSelf)
        {
            currentModel = Instantiate(data.modelPrefab, spawnPoint.position, spawnPoint.rotation);
            currentModel.transform.SetParent(spawnPoint);
            currentModel.transform.localPosition = Vector3.zero;
            currentModel.transform.localRotation = Quaternion.identity;
        }

        
        bool isUnlocked = PlayerDataManager.Instance.IsCharacterUnlocked(data.characterName);
        string currentSelected = PlayerDataManager.Instance.GetSelectedCharacter();

        if (isUnlocked)
        {
            //ĐÃ MUA
            btnBuyGold.gameObject.SetActive(false);
            btnBuyDiamond.gameObject.SetActive(false);

            
            btnEquip.gameObject.SetActive(true);

            
            if (currentSelected == data.characterName)
            {
                txtEquip.text = "SELECTED";
                btnEquip.interactable = false;
            }
            else
            {
                txtEquip.text = "SELECT";
                btnEquip.interactable = true;
            }
        }
        else
        {
            //CHƯA MUA
            btnBuyGold.gameObject.SetActive(true);
            btnBuyDiamond.gameObject.SetActive(true);

            
            btnEquip.gameObject.SetActive(false);

            
            txtGoldPrice.text = data.price.ToString("N0");
            txtDiamondPrice.text = data.diamondPrice.ToString("N0");
        }
    }


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

    public void BuyDiamondPack(int amount)
    {
        PlayerDataManager.Instance.AddDiamonds(amount);
        UpdateCurrencyUI();
        Debug.Log("Đã nạp " + amount + " kim cương!");
    }

    public void UpdateCurrencyUI()
    {
        if (txtTotalCoins != null)
            txtTotalCoins.text = PlayerDataManager.Instance.GetTotalCoins().ToString("N0");

        if (txtTotalDiamonds != null)
            txtTotalDiamonds.text = PlayerDataManager.Instance.GetTotalDiamonds().ToString("N0");
    }

    public void OnNextClick()
    {
        currentIndex++; 
        if (currentIndex >= allCharacters.Length)
        {
            currentIndex = 0;
        }
        UpdateShopUI(currentIndex);
    }

    public void OnPrevClick()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = allCharacters.Length - 1;
        }
        UpdateShopUI(currentIndex);
    }

    public void BackToMenu() { SceneManager.LoadScene("Menu"); }
}