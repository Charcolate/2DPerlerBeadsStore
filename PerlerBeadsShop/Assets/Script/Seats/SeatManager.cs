using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    [Header("Seats Container")]
    [SerializeField] private Transform seatsParent; 
    
    private List<Seat> allSeats = new List<Seat>();
    
    private void Start()
    { 
        // Auto-find all seats
        if (seatsParent != null)
        {
            Seat[] seats = seatsParent.GetComponentsInChildren<Seat>();
            allSeats.AddRange(seats);
        }
        else
        {
            Seat[] seats = FindObjectsOfType<Seat>();
            allSeats.AddRange(seats);
        }
        
        Debug.Log($"找到 {allSeats.Count} 个座位");
    }
    
    public bool HasEmptySeats()
    {
        foreach (Seat seat in allSeats)
        {
            if (!seat.IsOccupied)
                return true;
        }
        return false;
    }
    
    public Transform GetRandomEmptySeat()
    {
        List<Seat> emptySeats = new List<Seat>();
        
        foreach (Seat seat in allSeats)
        {
            if (!seat.IsOccupied)
                emptySeats.Add(seat);
        }
        
        if (emptySeats.Count == 0) return null;
        
        Seat randomSeat = emptySeats[Random.Range(0, emptySeats.Count)];
        randomSeat.Occupy();
        return randomSeat.transform;
    }
    
    public void ReleaseSeat(Seat seat)
    {
        seat.Vacate();
    }
}