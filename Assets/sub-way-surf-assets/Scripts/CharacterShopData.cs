using UnityEngine;

// Dòng này giúp Hải có thể chuột phải trong Project để tạo file Data mới
[CreateAssetMenu(fileName = "NewCharacter", menuName = "Shop/Character Data")]
public class CharacterShopData : ScriptableObject
{
    public string characterName;   // Tên nhân vật (Ninja, Zombie...)
    public int price;              // Giá tiền
    public GameObject modelPrefab; // Kéo file 3D của nhân vật vào đây
    public Sprite icon;            // Ảnh đại diện để hiện trong ô chọn
}