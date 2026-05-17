using UnityEngine;

public class DustSpawner : MonoBehaviour
{
    [Header("Dust Settings")]
    public GameObject dustPrefab;
    public int minDust = 3;
    public int maxDust = 8;
    public float dustSize = 0.5f;
    
    [Header("Spawning Area")]
    public Vector2 spawnAreaMin = new Vector2(-8, -4);
    public Vector2 spawnAreaMax = new Vector2(8, 4);
    public Color spawnAreaColor = new Color(0, 1, 0, 0.2f); // Green transparent
    public Color spawnAreaBorderColor = Color.green;
    
    void Start()
    {
        SpawnRandomDust();
    }

    public void SpawnRandomDust()
    {
        ClearDust();
        
        int dustCount = Random.Range(minDust, maxDust + 1);
        
        for (int i = 0; i < dustCount; i++)
        {
            SpawnDustParticle();
        }
    }
    
    void SpawnDustParticle()
    {
        Vector2 randomPosition = GetRandomPosition();
        
        GameObject dust = Instantiate(dustPrefab, randomPosition, Quaternion.identity);
        dust.transform.localScale = Vector3.one * dustSize;
        
        // Setup CircleCollider2D
        CircleCollider2D collider = dust.GetComponent<CircleCollider2D>();
        if (collider == null)
        {
            collider = dust.AddComponent<CircleCollider2D>();
        }
        collider.isTrigger = true;
        collider.radius = 0.5f;
        
        // Setup Rigidbody2D
        Rigidbody2D rb = dust.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = dust.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        
        // Random rotation
        dust.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        
        // Tag
        dust.tag = "Dust";
    }
    
    Vector2 GetRandomPosition()
    {
        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
        
        return new Vector2(randomX, randomY);
    }
    
    void ClearDust()
    {
        GameObject[] existingDust = GameObject.FindGameObjectsWithTag("Dust");
        foreach (GameObject dust in existingDust)
        {
            Destroy(dust);
        }
    }
    
    // Draw the spawn area in the editor
    void OnDrawGizmos()
    {
        // Calculate the area
        Vector2 center = (spawnAreaMin + spawnAreaMax) / 2f;
        Vector2 size = spawnAreaMax - spawnAreaMin;
        
        // Draw filled area
        Gizmos.color = spawnAreaColor;
        Gizmos.DrawCube(center, size);
        
        // Draw border
        Gizmos.color = spawnAreaBorderColor;
        DrawWireRectangle(center, size);
    }
    
    // Draw wireframe rectangle (Gizmos.DrawWireCube doesn't exist for 2D rectangle in all versions)
    void DrawWireRectangle(Vector2 center, Vector2 size)
    {
        float halfWidth = size.x / 2f;
        float halfHeight = size.y / 2f;
        
        Vector2 topLeft = new Vector2(center.x - halfWidth, center.y + halfHeight);
        Vector2 topRight = new Vector2(center.x + halfWidth, center.y + halfHeight);
        Vector2 bottomLeft = new Vector2(center.x - halfWidth, center.y - halfHeight);
        Vector2 bottomRight = new Vector2(center.x + halfWidth, center.y - halfHeight);
        
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}