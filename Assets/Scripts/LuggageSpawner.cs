using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XRayImage
{
    public Sprite image;
    public bool isContraband;
}

public class LuggageSpawner : MonoBehaviour
{
    public static LuggageSpawner Instance { get; private set; }

    public List<GameObject> luggagePrefabs = new List<GameObject>();
    public List<XRayImage> xRayImages = new List<XRayImage>();

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
        SecurityLuggage securityLuggage = luggage.GetComponent<SecurityLuggage>();
        luggagesSpawned++;
        securityLuggage.luggageID = luggagesSpawned;

        // Populate its x-ray image list with random x-ray images from the spawner
        for (int i = 0; i < 3; i++)
        {
            int randomImageIndex = Random.Range(0, xRayImages.Count);
            securityLuggage.xRayImages.Add(xRayImages[randomImageIndex]);

            // If that image is contraband, set the luggage's hasContraband bool to true
            if (xRayImages[randomImageIndex].isContraband)
            {
                securityLuggage.hasContraband = true;
            }
        }
    }
}
