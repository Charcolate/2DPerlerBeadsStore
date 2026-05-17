using UnityEngine;

public class MainDuster : MonoBehaviour
{
    [Header("Exclamation Settings")]
    public GameObject exclamationMark;
    public float timeUntilExclamation = 10f;
    
    private float timer;
    private bool exclamationShowing = false;
    
    void Start()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }
        
        timer = timeUntilExclamation;
    }
    
    void Update()
    {
        if (!exclamationShowing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                ShowExclamation();
            }
        }
    }
    
    void ShowExclamation()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(true);
            exclamationShowing = true;
            Debug.Log("Duster needs cleaning!");
        }
    }
    
    public void OnExclamationClicked()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }
        
        exclamationShowing = false;
        timer = timeUntilExclamation;
        
        Debug.Log("Timer reset! Exclamation will appear again in " + timeUntilExclamation + " seconds.");
    }
}