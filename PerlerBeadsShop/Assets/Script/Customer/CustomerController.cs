using System.Collections;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float waitBeforeOrder = 5f;
    
    [Header("Re-order Settings")]
    [SerializeField] private bool enableReOrder = true;
    [SerializeField] [Range(0f, 1f)] private float reOrderChance = 1f;
    [SerializeField] private float reOrderDelay = 3f;
    [SerializeField] private int maxReOrders = 3;
    
    [Header("Leave Settings")]
    [SerializeField] private float autoLeaveDelay = 5f;
    
    private Transform targetSeat;
    private Seat seatComponent;
    private GameObject exclamationMark;
    private bool isLeaving = false;
    private int reOrderCount = 0;
    private Vector3 spawnPosition; // Store where they came from
    
    private void Awake()
    {
        // Store spawn position immediately when created
        spawnPosition = transform.position;
        
        exclamationMark = transform.Find("感叹号")?.gameObject;
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
            
            BoxCollider2D col = exclamationMark.GetComponent<BoxCollider2D>();
            if (col == null)
            {
                col = exclamationMark.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
            }
            col.size = new Vector2(1f, 1f);
            
            ExclamationClick click = exclamationMark.GetComponent<ExclamationClick>();
            if (click == null)
            {
                click = exclamationMark.AddComponent<ExclamationClick>();
            }
        }
        else
        {
            Debug.LogError($"Cannot find 感叹号 in {gameObject.name}!");
        }
    }
    
    public void SetTargetSeat(Transform seat)
    {
        targetSeat = seat;
        seatComponent = seat.GetComponent<Seat>();
        spawnPosition = transform.position; // Make sure spawn position is correct
        StartCoroutine(WalkToSeatRoutine());
    }
    
    IEnumerator WalkToSeatRoutine()
    {
        Debug.Log($"[{gameObject.name}] Walking to seat from spawn: {spawnPosition}");
        
        while (targetSeat != null && Vector2.Distance(transform.position, targetSeat.position) > 0.1f)
        {
            if (isLeaving) yield break;
            
            Vector2 direction = (targetSeat.position - transform.position).normalized;
            float step = walkSpeed * Time.deltaTime;
            
            if (step > Vector2.Distance(transform.position, targetSeat.position))
            {
                step = Vector2.Distance(transform.position, targetSeat.position);
            }
            
            transform.Translate(direction * step);
            yield return null;
        }
        
        if (targetSeat != null && !isLeaving)
        {
            transform.position = targetSeat.position;
            Debug.Log($"[{gameObject.name}] Seated! Waiting {waitBeforeOrder}s");
            
            yield return new WaitForSeconds(waitBeforeOrder);
            
            if (!isLeaving)
            {
                ShowExclamation();
            }
        }
    }
    
    void ShowExclamation()
    {
        if (exclamationMark != null && !isLeaving)
        {
            exclamationMark.SetActive(true);
            Debug.Log($"[{gameObject.name}] ! APPEARED (Order #{reOrderCount + 1})");
        }
    }
    
    public void HideExclamation()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }
    }
    
    public void OnOrderTaken()
    {
        reOrderCount++;
        Debug.Log($"[{gameObject.name}] Order #{reOrderCount} taken");
        
        if (enableReOrder && !isLeaving)
        {
            if (maxReOrders == -1 || reOrderCount < maxReOrders)
            {
                float randomValue = Random.Range(0f, 1f);
                Debug.Log($"[{gameObject.name}] Re-order roll: {randomValue:F2}/{reOrderChance:F2} ({reOrderCount}/{maxReOrders})");
                
                if (randomValue <= reOrderChance)
                {
                    Debug.Log($"[{gameObject.name}] Will re-order in {reOrderDelay}s");
                    StartCoroutine(ReOrderRoutine());
                    return;
                }
                else
                {
                    Debug.Log($"[{gameObject.name}] Failed re-order chance - will leave in {autoLeaveDelay}s");
                    StartCoroutine(AutoLeaveRoutine());
                    return;
                }
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Max re-orders reached ({reOrderCount}/{maxReOrders}) - will leave in {autoLeaveDelay}s");
                StartCoroutine(AutoLeaveRoutine());
                return;
            }
        }
        else
        {
            Debug.Log($"[{gameObject.name}] Re-order disabled - will leave in {autoLeaveDelay}s");
            StartCoroutine(AutoLeaveRoutine());
        }
    }
    
    IEnumerator ReOrderRoutine()
    {
        yield return new WaitForSeconds(reOrderDelay);
        
        if (!isLeaving)
        {
            ShowExclamation();
        }
    }
    
    IEnumerator AutoLeaveRoutine()
    {
        yield return new WaitForSeconds(autoLeaveDelay);
        
        if (!isLeaving)
        {
            Debug.Log($"[{gameObject.name}] Auto-leaving after delay");
            Leave();
        }
    }
    
    [ContextMenu("LEAVE")]
    public void LeaveRestaurantButton()
    {
        Debug.Log($"[{gameObject.name}] BUTTON: Leave clicked!");
        Leave();
    }
    
    public void Leave()
    {
        if (!isLeaving)
        {
            Debug.Log($"[{gameObject.name}] Leave() - Walking back to spawn: {spawnPosition}");
            isLeaving = true;
            
            HideExclamation();
            
            StopAllCoroutines();
            
            if (seatComponent != null)
            {
                seatComponent.Vacate();
            }
            
            StartCoroutine(WalkBackToSpawnRoutine());
        }
    }
    
    IEnumerator WalkBackToSpawnRoutine()
    {
        Debug.Log($"[{gameObject.name}] Walking back from {transform.position} to spawn: {spawnPosition}");
        
        // Keep walking until reaching spawn point
        while (Vector2.Distance(transform.position, spawnPosition) > 0.05f)
        {
            Vector2 direction = (spawnPosition - transform.position).normalized;
            float step = walkSpeed * Time.deltaTime;
            
            // Don't overshoot
            float distanceLeft = Vector2.Distance(transform.position, spawnPosition);
            if (step > distanceLeft)
            {
                step = distanceLeft;
            }
            
            transform.Translate(direction * step);
            yield return null;
        }
        
        // Snap to exact spawn position
        transform.position = spawnPosition;
        
        Debug.Log($"[{gameObject.name}] Reached spawn point - Destroying");
        Destroy(gameObject);
    }
}