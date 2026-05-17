using UnityEngine;

public class ExclamationClick : MonoBehaviour
{
    private CustomerController customer;
    private OrderPopup popup;
    
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
        // Check if popup is already open
        if (popup != null && popup.IsOpen())
        {
            Debug.Log("Popup already open, ignoring click on exclamation");
            return;
        }
        
        Debug.Log("Exclamation clicked!");
        
        // Hide this exclamation
        gameObject.SetActive(false);
        
        // Tell customer order was taken
        if (customer != null)
        {
            customer.OnOrderTaken();
        }
        
        // Show popup
        if (popup != null)
        {
            popup.Show();
        }
    }
}