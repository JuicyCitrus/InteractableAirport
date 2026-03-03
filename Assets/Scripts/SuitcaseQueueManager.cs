using System.Collections.Generic;
using UnityEngine;

public class SuitcaseQueueManager : MonoBehaviour
{
    [SerializeField] private CargoWinLoseController winLose;

    [Header("Spawning")]
    [SerializeField] private List<GameObject> suitcasePrefabs = new List<GameObject>();
    [SerializeField] private int initialQueueCount = 6;

    [Header("Queue Layout (World)")]
    [SerializeField] private Transform queueRoot;
    [SerializeField] private Vector3 queueStep = new Vector3(0f, 0f, -0.9f); // line going backwards
    [SerializeField] private float queueLerpSpeed = 10f;

    [Header("Pickup Spot")]
    [SerializeField] private Transform pickupSpot;
    [SerializeField] private float pickupLerpSpeed = 12f;
    [SerializeField] private float pickupArriveDistance = 0.02f;

    [Header("Selection Gating")]
    [Tooltip("Colliders will be enabled only when the suitcase reaches the pickup spot.")]
    [SerializeField] private bool enableCollidersOnlyAtPickup = true;

    private readonly List<SuitcaseItem> queue = new List<SuitcaseItem>();
    private SuitcaseItem activeAtPickup;

    public int InitialQueueCount => initialQueueCount;
    private void Awake()
    {
        if (queueRoot == null) queueRoot = transform;
    }

    private void Start()
    {
        if (winLose != null)
            winLose.SetTargetPlacements(initialQueueCount);

        // Fill the queue
        for (int i = 0; i < initialQueueCount; i++)
            EnqueueRandom();

        // Start presenting the first suitcase
        PromoteNextToPickupIfNeeded();
        ApplyColliderGating();
    }

    private void Update()
    {
        // Smoothly place the visible queue
        for (int i = 0; i < queue.Count; i++)
        {
            SuitcaseItem item = queue[i];
            if (item == null) continue;

            Vector3 targetPos = queueRoot.position + queueStep * i;
            Quaternion targetRot = queueRoot.rotation;

            float t = 1f - Mathf.Exp(-queueLerpSpeed * Time.deltaTime);
            item.transform.SetPositionAndRotation(
                Vector3.Lerp(item.transform.position, targetPos, t),
                Quaternion.Slerp(item.transform.rotation, targetRot, t)
            );
        }

        // Lerp the "front" suitcase to the pickup spot
        if (activeAtPickup != null && pickupSpot != null)
        {
            float t = 1f - Mathf.Exp(-pickupLerpSpeed * Time.deltaTime);
            activeAtPickup.transform.SetPositionAndRotation(
                Vector3.Lerp(activeAtPickup.transform.position, pickupSpot.position, t),
                Quaternion.Slerp(activeAtPickup.transform.rotation, pickupSpot.rotation, t)
            );

            // When it's close enough, enable colliders so player can pick it
            if (enableCollidersOnlyAtPickup)
            {
                if (Vector3.Distance(activeAtPickup.transform.position, pickupSpot.position) <= pickupArriveDistance)
                    activeAtPickup.SetCollidersEnabled(true);
            }
        }
    }

    public SuitcaseItem GetCurrentPickupSuitcase() => activeAtPickup;

    /// <summary>
    /// Called by SuitcasePlacer when the player picks up the suitcase (BeginPreview).
    /// </summary>
    public void NotifyPickedUp(SuitcaseItem item)
    {
        if (item == null) return;

        // If they picked the pickup suitcase, advance the queue
        if (item == activeAtPickup)
        {
            activeAtPickup = null;
            PromoteNextToPickupIfNeeded();
            ApplyColliderGating();

            // Keep queue stocked
            EnqueueRandom();
        }
    }

    private void PromoteNextToPickupIfNeeded()
    {
        if (activeAtPickup != null) return;
        if (queue.Count == 0) return;

        // front of the queue is index 0 (closest to pickup)
        activeAtPickup = queue[0];
        queue.RemoveAt(0);
    }

    private void ApplyColliderGating()
    {
        if (!enableCollidersOnlyAtPickup) return;

        // Disable colliders for everything not in pickup position
        for (int i = 0; i < queue.Count; i++)
        {
            if (queue[i] != null) queue[i].SetCollidersEnabled(false);
        }

        if (activeAtPickup != null)
            activeAtPickup.SetCollidersEnabled(false); // will enable on arrival
    }

    private void EnqueueRandom()
    {
        if (suitcasePrefabs == null || suitcasePrefabs.Count == 0) return;

        int idx = Random.Range(0, suitcasePrefabs.Count);
        GameObject prefab = suitcasePrefabs[idx];
        if (prefab == null) return;

        Vector3 spawnPos = queueRoot.position + queueStep * queue.Count;
        Quaternion spawnRot = queueRoot.rotation;

        GameObject go = Instantiate(prefab, spawnPos, spawnRot);
        SuitcaseItem item = go.GetComponent<SuitcaseItem>();
        if (item == null) item = go.AddComponent<SuitcaseItem>(); // safety for prototyping

        // Start disabled so it doesn't block raycasts until it's active
        if (enableCollidersOnlyAtPickup)
            item.SetCollidersEnabled(false);

        queue.Add(item);
    }
}