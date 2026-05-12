using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer Prefabs")]
    [SerializeField] private GameObject[] customerPrefabs;
    
    [Header("Spawn Settings")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minSpawnDelay = 3f;
    [SerializeField] private float maxSpawnDelay = 8f;
    
    [Header("Seating System")]
    [SerializeField] private SeatManager seatManager;
    
    private void Start()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Spawn point not assigned! Assign 出生点 transform.");
        }
        
        if (seatManager == null)
        {
             seatManager = FindFirstObjectByType<SeatManager>();
        }
        
        StartCoroutine(SpawnRoutine());
    }
    
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);
            
            if (seatManager != null && seatManager.HasEmptySeats())
            {
                SpawnCustomer();
            }
            else
            {
                Debug.Log("所有座位已满，停止生成客人！");
                yield break;
            }
        }
    }
    
    private void SpawnCustomer()
    {
        if (customerPrefabs.Length == 0) return;
        
        Transform emptySeat = seatManager.GetRandomEmptySeat();
        if (emptySeat == null) return;
        
        GameObject customerPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Length)];
        
        Vector3 spawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        GameObject customer = Instantiate(customerPrefab, spawnPos, Quaternion.identity);
        
        Transform parent = GameObject.Find("客人")?.transform;
        if (parent != null)
        {
            customer.transform.SetParent(parent);
        }
        
        CustomerController controller = customer.GetComponent<CustomerController>();
        if (controller != null)
        {
            controller.SetTargetSeat(emptySeat);
        }
    }
}