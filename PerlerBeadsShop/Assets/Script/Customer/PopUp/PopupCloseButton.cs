using UnityEngine;

public class PopupCloseButton : MonoBehaviour
{
    [SerializeField] private OrderPopup orderPopup;
    
    private void Start()
    {
        if (orderPopup == null)
        {
            orderPopup = GetComponentInParent<OrderPopup>();
        }
    }
    
    private void OnMouseDown()
    {
        if (orderPopup != null)
        {
            orderPopup.Hide();
        }
    }
}