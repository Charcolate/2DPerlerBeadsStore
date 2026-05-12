using UnityEngine;
using UnityEngine.UI;

public class OrderPopup : MonoBehaviour
{
    [Header("Popup UI")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private Button closeButton;
    
    private void Awake()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }
    
    public void Show()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
    }
    
    public void Hide()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }
}