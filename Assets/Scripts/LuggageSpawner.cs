using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class XRayImage
{
    public Sprite image;
    public float spriteResizeMultiplier;
}

public class LuggageSpawner : MonoBehaviour
{
    public static LuggageSpawner Instance { get; private set; }

    public List<GameObject> luggagePrefabs = new List<GameObject>();
    public List<XRayImage> xRayImages = new List<XRayImage>();
    public List<XRayImage> contrabandXRayImages = new List<XRayImage>();

    [Header("Bag Details")]
    public int numberOfItemsPerBag = 6;
    [Range(0, 1)] public float contrabandChance = 0.3f;

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

        // Determine if it will have contraband
        bool willHaveContraband = Random.value <= contrabandChance;

        // Populate its x-ray image list with random x-ray images from the spawner
        for (int i = 0; i < numberOfItemsPerBag; i++)
        {
            // Add a contraband item right away if it should have one and does not have one yet
            if(willHaveContraband && securityLuggage.hasContraband == false)
            {
                int randomContrabandImageIndex = Random.Range(0, contrabandXRayImages.Count);
                securityLuggage.xRayImages.Add(contrabandXRayImages[randomContrabandImageIndex]);
                securityLuggage.hasContraband = true;
            }
            else
            {
                int randomImageIndex = Random.Range(0, xRayImages.Count);
                securityLuggage.xRayImages.Add(xRayImages[randomImageIndex]);
            }
        }
    }
}
