using UnityEngine;

public class ExclamationClick : MonoBehaviour
{
    private CustomerController customerController;
    
    [Header("Popup Reference")]
    [SerializeField] private OrderPopup orderPopup; 
    
    private void Start()
    {
        customerController = GetComponentInParent<CustomerController>();
    
        // Debug: Find ALL active OrderPopup in scene
        OrderPopup[] allPopups = FindObjectsByType<OrderPopup>(FindObjectsSortMode.None);
        Debug.Log($"找到 {allPopups.Length} 个 OrderPopup 组件");
    
        foreach (OrderPopup popup in allPopups)
        {
            Debug.Log($"OrderPopup 在: {popup.gameObject.name}, Active: {popup.gameObject.activeSelf}");
        }
    
        if (orderPopup == null)
        {
            orderPopup = FindFirstObjectByType<OrderPopup>();
        }
    
        if (orderPopup == null)
        {
            Debug.LogError("找不到 OrderPopup！确认 弹窗 是 Active 状态且有 OrderPopup 脚本");
        }
        else
        {
            Debug.Log($"成功找到弹窗: {orderPopup.gameObject.name}");
        }
    }
    
    private void OnMouseDown()
    {
        Debug.Log("感叹号被点击！");
        
        // Hide the exclamation
        if (customerController != null)
        {
            customerController.HideExclamation();
        }
        
        // Show popup screen
        if (orderPopup != null)
        {
            orderPopup.Show();
        }
        else
        {
            Debug.LogWarning("没有找到 OrderPopup！");
        }
    }
}