using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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
    
    private void Update()
    {
        // Close popup when ESC is pressed
        if (popupPanel != null && popupPanel.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Hide();
            }
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