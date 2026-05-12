using UnityEngine;

public class Seat : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;
    
    public void Occupy()
    {
        IsOccupied = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.gray;
        }
    }
    
    public void Vacate()
    {
        IsOccupied = false;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.white;
        }
    }
}