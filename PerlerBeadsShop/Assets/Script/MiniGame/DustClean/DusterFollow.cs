using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class DusterFollow : MonoBehaviour
{
    private Camera mainCamera;
    
    [Header("Boundary Settings")]
    public bool constrainToArea = true;
    public Vector2 areaMin = new Vector2(-8, -4);
    public Vector2 areaMax = new Vector2(8, 4);
    public float dusterOffset = 0.5f;
    
    private bool gameActive = true;
    private bool dustSpawned = false;
    
    void Start()
    {
        mainCamera = Camera.main;
        
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        
        gameActive = true;
        dustSpawned = false;
        
        Invoke(nameof(StartChecking), 0.5f);
    }
    
    void StartChecking()
    {
        GameObject[] dust = GameObject.FindGameObjectsWithTag("Dust");
        if (dust.Length > 0)
        {
            dustSpawned = true;
            Debug.Log($"Dust spawned! Total dust: {dust.Length}");
        }
    }

    void Update()
    {
        if (!gameActive) return;
        
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        
        if (constrainToArea)
        {
            worldPosition = ClampPositionToArea(worldPosition);
        }
        
        transform.position = worldPosition;
        
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EndGame(false);
        }
        
        if (dustSpawned)
        {
            CheckAllDustCleaned();
        }
    }
    
    Vector2 ClampPositionToArea(Vector2 position)
    {
        float minX = areaMin.x + dusterOffset;
        float maxX = areaMax.x - dusterOffset;
        float minY = areaMin.y + dusterOffset;
        float maxY = areaMax.y - dusterOffset;
        
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        
        return position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dust") && gameActive)
        {
            Destroy(other.gameObject);
            Debug.Log("Dust cleaned!");
        }
    }
    
    void CheckAllDustCleaned()
    {
        GameObject[] remainingDust = GameObject.FindGameObjectsWithTag("Dust");
        
        if (remainingDust.Length == 0)
        {
            EndGame(true);
        }
    }
    
    void EndGame(bool success)
    {
        gameActive = false;
        
        if (success)
            Debug.Log("✅ SUCCESS! All dust has been cleaned!");
        else
            Debug.Log("❌ FAILED!");
        
        // Find and reset the main duster's exclamation
        MainDuster mainDuster = FindFirstObjectByType<MainDuster>();
        if (mainDuster != null)
        {
            mainDuster.OnExclamationClicked();
        }
        
        // Destroy the whole mini-game
        Destroy(transform.parent.gameObject);
    }
}