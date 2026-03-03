using System.Collections.Generic;
using UnityEngine;

public class LuggageSpawner : MonoBehaviour
{
    public static LuggageSpawner Instance { get; private set; }

    public List<GameObject> luggagePrefabs = new List<GameObject>();

    public Vector3 spawnPosition;

    public int luggagesSpawned = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SpawnLuggage();
    }

    public void SpawnLuggage()
    {
        // Don't spawn anymore luggage once the per round limit has been reached
        if(luggagesSpawned >= SecurityScoring.Instance.luggageInRound)
        {
            return;
        }

        // Choose a rabndom luggage prefab from the list
        int randomIndex = Random.Range(0, luggagePrefabs.Count);

        // Spawn it at the spawn position
        GameObject luggage = Instantiate(luggagePrefabs[randomIndex], spawnPosition, this.transform.rotation);
        luggagesSpawned++;
        luggage.GetComponent<SecurityLuggage>().luggageID = luggagesSpawned;
    }
}
