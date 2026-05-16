using UnityEngine;
using UnityEngine.UI;

public class DustSpawner : MonoBehaviour
{
    [Header("Dust Settings")]
    public GameObject DustPrefab; 
    public Transform DustParent; 
    public int minDusts = 3;
    public int maxDusts = 8;
    public float DustSize = 50f;
    
    [Header("Spawning Area Padding")]
    public float padding = 50f; 
    
    private Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (DustParent == null)
            DustParent = transform;
            
        SpawnRandomDusts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnRandomDusts()
    {
        ClearDusts();
        
        int DustCount = Random.Range(minDusts, maxDusts + 1);
        
        for (int i = 0; i < DustCount; i++)
        {
            SpawnDust();
        }
    }
    
    void SpawnDust()
    {
        Vector2 randomPosition = GetRandomScreenPosition();
        
        GameObject Dust = Instantiate(DustPrefab, randomPosition, Quaternion.identity, DustParent);
        
        // Setup RectTransform
        RectTransform rectTransform = Dust.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(DustSize, DustSize);
        }
        
        // Setup Dust Collider
        BoxCollider2D collider = Dust.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = Dust.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
                
    }
    
    Vector2 GetRandomScreenPosition()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        
        float randomX = Random.Range(padding, screenWidth - padding);
        float randomY = Random.Range(padding, screenHeight - padding);
        
        return new Vector2(randomX, randomY);
    }
    
    void ClearDusts()
    {
        foreach (Transform child in DustParent)
        {
            Destroy(child.gameObject);
        }
    }
}
