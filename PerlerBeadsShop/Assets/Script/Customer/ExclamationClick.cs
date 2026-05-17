using UnityEngine;
using System.Collections.Generic;

public class ExclamationClick : MonoBehaviour
{
    private CustomerController customer;
    private OrderPopup popup;
    
    [Header("Mini-Game Settings")]
    public List<GameObject> possibleMiniGames = new List<GameObject>(); // Drag prefabs here
    
    void Start()
    {
        customer = GetComponentInParent<CustomerController>();
        popup = FindFirstObjectByType<OrderPopup>();
        
        if (popup == null)
        {
            Debug.LogError("OrderPopup not found in scene!");
        }
    }
    
    void OnMouseDown()
    {
        if (popup != null && popup.IsOpen())
        {
            Debug.Log("Popup already open, ignoring click on exclamation");
            return;
        }
        
        Debug.Log("Exclamation clicked!");
        
        gameObject.SetActive(false);
        
        if (customer != null)
        {
            customer.OnOrderTaken();
        }
        
        if (popup != null)
        {
            popup.Show();
        }
        
        StartRandomMiniGame();
    }
    
    void StartRandomMiniGame()
    {
        if (possibleMiniGames.Count > 0)
        {
            int randomIndex = Random.Range(0, possibleMiniGames.Count);
            GameObject selectedMiniGame = possibleMiniGames[randomIndex];
            
            if (selectedMiniGame != null)
            {
                // Instantiate the prefab (brand new copy each time)
                Instantiate(selectedMiniGame);
                Debug.Log($"Started mini-game: {selectedMiniGame.name}");
            }
        }
    }
}