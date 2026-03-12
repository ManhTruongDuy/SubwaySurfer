using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Cài đặt")]
    public Transform playerRoot;           
    public Character playerScript;         
    public GameObject[] allCharacterPrefabs; 

    private void Awake()
    {
        SpawnCharacter();
    }

    void SpawnCharacter()
    {
        
        string selectedName = PlayerDataManager.Instance.GetSelectedCharacter();

        
        GameObject prefabToSpawn = null;
        foreach (var prefab in allCharacterPrefabs)
        {
            
            if (prefab.name == selectedName)
            {
                prefabToSpawn = prefab;
                break;
            }
        }

        
        if (prefabToSpawn == null && allCharacterPrefabs.Length > 0)
        {
            prefabToSpawn = allCharacterPrefabs[0];
        }

        if (prefabToSpawn != null)
        {
            
            GameObject newModel = Instantiate(prefabToSpawn, playerRoot);

            
            newModel.transform.localPosition = Vector3.zero;
            newModel.transform.localRotation = Quaternion.identity;

            
            Animator newAnim = newModel.GetComponent<Animator>();
            if (newAnim == null) newAnim = newModel.GetComponentInChildren<Animator>();

            
            if (playerScript != null && newAnim != null)
            {
                playerScript.animator = newAnim;
            }
        }
    }
}