using UnityEngine;
using UnityEngine.InputSystem;

public class DusterFollow : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider != null)
        {
            collider.isTrigger = true;
            collider.size = rectTransform.sizeDelta;
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        rectTransform.position = mousePosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Dust"))
        {
            Destroy(other.gameObject);
        }
    }
}
