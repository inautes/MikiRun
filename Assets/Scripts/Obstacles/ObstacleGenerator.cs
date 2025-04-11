using UnityEngine;
using System.Collections.Generic;

public class ObstacleGenerator : MonoBehaviour
{
    [Header("Obstacle Settings")]
    public GameObject[] urbanObstaclePrefabs;
    public GameObject[] forestObstaclePrefabs;
    public GameObject[] desertObstaclePrefabs;
    public GameObject[] snowObstaclePrefabs;
    
    [Header("Spawn Settings")]
    public float spawnDistance = 50f;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public float lateralSpawnRange = 5f;
    public int maxObstaclesOnScreen = 15;
    
    private float nextSpawnTime;
    private Transform playerTransform;
    private GameManager gameManager;
    private List<GameObject> activeObstacles = new List<GameObject>();
    private int currentWorldIndex = 0;
    
    void Start()
    {
        gameManager = GameManager.Instance;
        playerTransform = FindObjectOfType<PlayerController>().transform;
        
        nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
    }
    
    void Update()
    {
        if (playerTransform == null || gameManager == null) return;
        
        currentWorldIndex = Mathf.FloorToInt(playerTransform.position.z / gameManager.distanceToChangeWorld) % 4;
        
        if (Time.time >= nextSpawnTime && activeObstacles.Count < maxObstaclesOnScreen)
        {
            SpawnObstacle();
            
            float interval = Mathf.Lerp(maxSpawnInterval, minSpawnInterval, 
                gameManager.GetCurrentGameSpeed() / gameManager.maxGameSpeed);
            nextSpawnTime = Time.time + Random.Range(interval * 0.8f, interval * 1.2f);
        }
        
        CleanupObstacles();
    }
    
    void SpawnObstacle()
    {
        if (playerTransform == null) return;
        
        float spawnZ = playerTransform.position.z + spawnDistance;
        float spawnX = Random.Range(-lateralSpawnRange, lateralSpawnRange);
        Vector3 spawnPosition = new Vector3(spawnX, 0, spawnZ);
        
        GameObject[] currentObstaclePrefabs = GetCurrentWorldObstacles();
        
        if (currentObstaclePrefabs.Length == 0) return;
        
        GameObject obstaclePrefab = currentObstaclePrefabs[Random.Range(0, currentObstaclePrefabs.Length)];
        
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        
        activeObstacles.Add(obstacle);
    }
    
    void CleanupObstacles()
    {
        if (playerTransform == null) return;
        
        for (int i = activeObstacles.Count - 1; i >= 0; i--)
        {
            if (activeObstacles[i] == null)
            {
                activeObstacles.RemoveAt(i);
                continue;
            }
            
            if (activeObstacles[i].transform.position.z < playerTransform.position.z - 20f)
            {
                Destroy(activeObstacles[i]);
                activeObstacles.RemoveAt(i);
            }
        }
    }
    
    GameObject[] GetCurrentWorldObstacles()
    {
        switch (currentWorldIndex)
        {
            case 0: // Urban
                return urbanObstaclePrefabs;
            case 1: // Forest
                return forestObstaclePrefabs;
            case 2: // Desert
                return desertObstaclePrefabs;
            case 3: // Snow
                return snowObstaclePrefabs;
            default:
                return urbanObstaclePrefabs;
        }
    }
}
