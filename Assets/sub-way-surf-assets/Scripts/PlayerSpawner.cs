using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Cài đặt")]
    public Transform playerRoot;           // Kéo cái vỏ PlayerRoot vào đây
    public Character playerScript;         // Kéo cái vỏ PlayerRoot vào đây (nó chứa script Character)
    public GameObject[] allCharacterPrefabs; // Danh sách Prefab nhân vật (phải giống hệt bên Menu/Shop)

    private void Awake()
    {
        SpawnCharacter();
    }

    void SpawnCharacter()
    {
        // 1. Lấy tên nhân vật đang chọn từ Shop
        string selectedName = PlayerDataManager.Instance.GetSelectedCharacter();

        // 2. Tìm Prefab tương ứng
        GameObject prefabToSpawn = null;
        foreach (var prefab in allCharacterPrefabs)
        {
            // Quan trọng: Tên Prefab phải trùng tên đã lưu
            if (prefab.name == selectedName)
            {
                prefabToSpawn = prefab;
                break;
            }
        }

        // Nếu không tìm thấy (lỗi tên) thì lấy con đầu tiên làm mặc định để chống crash
        if (prefabToSpawn == null && allCharacterPrefabs.Length > 0)
        {
            prefabToSpawn = allCharacterPrefabs[0];
        }

        if (prefabToSpawn != null)
        {
            // 3. Tạo nhân vật LÀM CON của PlayerRoot
            GameObject newModel = Instantiate(prefabToSpawn, playerRoot);

            // Chỉnh vị trí về 0,0,0 để nó đứng đúng giữa tâm PlayerRoot
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.identity;

            // 4. KẾT NỐI ANIMATOR
            // Tìm Animator trên người con mới sinh ra
            Animator newAnim = newModel.GetComponent<Animator>();
            if (newAnim == null) newAnim = newModel.GetComponentInChildren<Animator>();

            // Gửi Animator này cho script Character điều khiển
            if (playerScript != null && newAnim != null)
            {
                playerScript.animator = newAnim;
            }
        }
    }
}