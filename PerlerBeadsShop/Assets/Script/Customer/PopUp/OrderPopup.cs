using UnityEngine;
using UnityEngine.InputSystem;

public class OrderPopup : MonoBehaviour
{
    [SerializeField] private GameObject popupObject;
    
    private bool isOpen = false;
    
    void Awake()
    {
        if (popupObject != null)
        {
            popupObject.SetActive(false);
        }
    }
    
    void Update()
    {
        // Works with new Input System
        if (isOpen && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("ESC pressed - closing popup");
            Hide();
        }
    }
    
    public void Show()
    {
        if (popupObject != null)
        {
            popupObject.SetActive(true);
            isOpen = true;
            Debug.Log("POPUP SHOWN");
        }
    }
    
    public void Hide()
    {
        if (popupObject != null)
        {
            popupObject.SetActive(false);
            isOpen = false;
            Debug.Log("POPUP HIDDEN");
        }
    }
    
    public bool IsOpen()
    {
        return isOpen;
    }
}