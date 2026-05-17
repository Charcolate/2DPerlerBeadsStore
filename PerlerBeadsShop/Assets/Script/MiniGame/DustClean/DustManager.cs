using UnityEngine;
using UnityEngine.Events;

public class DustManager : MonoBehaviour
{
    [Header("References")]
    public DustSpawner dustSpawner;
    public GameObject popupPanel; // The popup window containing the mini-game
    
    [Header("Events")]
    public UnityEvent onSuccess; // Triggered when all dust is cleaned
    public UnityEvent onFailed;  // Triggered when popup is closed prematurely
    
    [Header("State")]
    public bool isActive = false;
    public int totalDustCount = 0;
    public int cleanedDustCount = 0;
    
    private bool wasSuccessful = false;
    
    void Start()
    {
        if (dustSpawner == null)
            dustSpawner = FindObjectOfType<DustSpawner>();
            
        if (popupPanel == null)
            popupPanel = gameObject; // Assume this script is on the popup
    }
    
    public void StartMiniGame()
    {
        isActive = true;
        wasSuccessful = false;
        cleanedDustCount = 0;
        
        // Spawn dust
        if (dustSpawner != null)
        {
            dustSpawner.SpawnRandomDust();
            totalDustCount = GameObject.FindGameObjectsWithTag("Dust").Length;
        }
        
        // Show popup
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
        
        Debug.Log($"Mini-game started! Total dust: {totalDustCount}");
    }
    
    public void DustCleaned()
    {
        if (!isActive) return;
        
        cleanedDustCount++;
        Debug.Log($"Dust cleaned: {cleanedDustCount}/{totalDustCount}");
        
        // Check if all dust is cleaned
        if (cleanedDustCount >= totalDustCount)
        {
            Success();
        }
    }
    
    void Success()
    {
        if (!isActive) return;
        
        wasSuccessful = true;
        isActive = false;
        
        Debug.Log("All dust cleaned! Success!");
        
        // Trigger success event
        onSuccess?.Invoke();
        
        // Close popup
        ClosePopup();
    }
    
    public void ClosePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        
        // If popup closed before completing, it's a failure
        if (isActive && !wasSuccessful)
        {
            Failed();
        }
        
        isActive = false;
    }
    
    void Failed()
    {
        Debug.Log("Mini-game failed! Popup closed before finishing.");
        onFailed?.Invoke();
    }
    
    // Called by ESC key or close button
    void Update()
    {
        if (isActive && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }
    
    // Public method for close button
    public void OnCloseButtonClicked()
    {
        ClosePopup();
    }
}