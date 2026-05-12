using System.Collections;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float minWaitForOrder = 3f;
    [SerializeField] private float maxWaitForOrder = 8f;
    
    private Transform targetSeat;
    private Seat seatComponent;
    private GameObject exclamationMark;
    
    private void Awake()
    {
        // Find ! in children
        exclamationMark = transform.Find("感叹号")?.gameObject;
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
            
            // Make sure it has a collider for clicking
            if (exclamationMark.GetComponent<Collider2D>() == null)
            {
                BoxCollider2D col = exclamationMark.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
            }
            
            // Add click handler if not already there
            if (exclamationMark.GetComponent<ExclamationClick>() == null)
            {
                exclamationMark.AddComponent<ExclamationClick>();
            }
        }
    }
    
    public void SetTargetSeat(Transform seat)
    {
        targetSeat = seat;
        seatComponent = seat.GetComponent<Seat>();
        StartCoroutine(GoToSeat());
    }
    
    private IEnumerator GoToSeat()
    {
        // Walk to seat
        while (targetSeat != null && Vector2.Distance(transform.position, targetSeat.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                targetSeat.position,
                walkSpeed * Time.deltaTime
            );
            yield return null;
        }
        
            // Arrived at seat
        if (targetSeat != null)
        {
            transform.position = targetSeat.position;
            Debug.Log("客人已就座");
            
            // Wait random time, then show ！
            float waitTime = Random.Range(minWaitForOrder, maxWaitForOrder);
            yield return new WaitForSeconds(waitTime);
            
            ShowExclamation();
        }
    }
    
    private void ShowExclamation()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(true);
            Debug.Log("感叹号出现 - 客人需要服务！");
        }
    }
    
    public void HideExclamation()
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(false);
        }
    }
}