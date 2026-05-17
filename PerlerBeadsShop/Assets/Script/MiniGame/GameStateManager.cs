using UnityEngine;
using UnityEngine.Events;

public enum MiniGameState
{
    Inactive,
    Playing,
    Success,
    Failed
}

public class GameStateManager : MonoBehaviour
{
    [Header("State")]
    public MiniGameState currentState = MiniGameState.Inactive;
    public int successCount = 0;
    public int failCount = 0;
    
    [Header("Events")]
    public UnityEvent onGameSuccess;
    public UnityEvent onGameFailed;
    public UnityEvent<int> onSuccessCountChanged; // Passes new success count
    public UnityEvent<int> onFailCountChanged;    // Passes new fail count
    
    private DustManager dustManager;
    
    void Start()
    {
        dustManager = FindObjectOfType<DustManager>();
        
        // Subscribe to dust manager events
        if (dustManager != null)
        {
            dustManager.onSuccess.AddListener(HandleSuccess);
            dustManager.onFailed.AddListener(HandleFailure);
        }
    }
    
    public void HandleSuccess()
    {
        currentState = MiniGameState.Success;
        successCount++;
        onSuccessCountChanged?.Invoke(successCount);
        onGameSuccess?.Invoke();
        
        Debug.Log($"Mini-game SUCCESS! Total successes: {successCount}");
    }
    
    public void HandleFailure()
    {
        currentState = MiniGameState.Failed;
        failCount++;
        onFailCountChanged?.Invoke(failCount);
        onGameFailed?.Invoke();
        
        Debug.Log($"Mini-game FAILED! Total failures: {failCount}");
    }
    
    public void StartNewGame()
    {
        currentState = MiniGameState.Playing;
        
        if (dustManager != null)
        {
            dustManager.StartMiniGame();
        }
    }
    
    public void ResetCounters()
    {
        successCount = 0;
        failCount = 0;
        onSuccessCountChanged?.Invoke(0);
        onFailCountChanged?.Invoke(0);
    }
}