using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XRayImage
{
    public Sprite image;
    public float spriteResizeMultiplier;
    public bool isContraband;
}

public class LuggageSpawner : MonoBehaviour
{
    public static LuggageSpawner Instance { get; private set; }

    public List<GameObject> luggagePrefabs = new List<GameObject>();
    public List<XRayImage> xRayImages = new List<XRayImage>();

    [Header("Bag Details")]
    public int numberOfItemsPerBag = 6;

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

    public void SpawnLuggage()
    {

        // Choose a rabndom luggage prefab from the list
        int randomIndex = Random.Range(0, luggagePrefabs.Count);

        // Spawn it at the spawn position
        GameObject luggage = Instantiate(luggagePrefabs[randomIndex], transform.position, this.transform.rotation);
        SecurityLuggage securityLuggage = luggage.GetComponent<SecurityLuggage>();
        luggagesSpawned++;
        securityLuggage.luggageID = luggagesSpawned;

        // Populate its x-ray image list with random x-ray images from the spawner
        for (int i = 0; i < numberOfItemsPerBag; i++)
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
