using UnityEngine;

[DisallowMultipleComponent]
public class SuitcaseItem : MonoBehaviour
{
    [Header("Optional References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider[] cols;
    [SerializeField] private Renderer[] rends;

    [Header("Materials")]
    [SerializeField] private Material previewValidMat;
    [SerializeField] private Material previewInvalidMat;
    [SerializeField] private Material placedMat;

    [Header("Preview Clone")]
    [Tooltip("If true, we spawn a visual-only clone to use as the preview ghost.")]
    public bool usePreviewClone = true;

    private GameObject previewInstance;
    private Renderer[] previewRends;

    private bool isPreviewing;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (cols == null || cols.Length == 0) cols = GetComponentsInChildren<Collider>(true);
        if (rends == null || rends.Length == 0) rends = GetComponentsInChildren<Renderer>(true);
    }

    /// <summary>
    /// Enter preview mode:
    /// - Hides REAL suitcase visuals (prevents “duplicate” look)
    /// - Disables REAL colliders so it doesn't block rays
    /// - Spawns preview clone (visual only)
    /// </summary>
    public void BeginPreview()
    {
        isPreviewing = true;

        // Disable collisions so selection/aiming doesn’t get blocked
        SetCollidersEnabled(false);

        // Freeze physics while selected
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Hide the real suitcase visuals while previewing
        SetRenderersEnabled(false);

        if (!usePreviewClone)
            return;

        if (previewInstance == null)
        {
            previewInstance = Instantiate(gameObject);
            previewInstance.name = "[SuitcasePreviewClone]";

            // Strip gameplay from clone
            foreach (var c in previewInstance.GetComponentsInChildren<Collider>(true))
                c.enabled = false;

            var rbClone = previewInstance.GetComponent<Rigidbody>();
            if (rbClone) Destroy(rbClone);

            var itemClone = previewInstance.GetComponent<SuitcaseItem>();
            if (itemClone) Destroy(itemClone);

            previewRends = previewInstance.GetComponentsInChildren<Renderer>(true);

            foreach(MeshRenderer r in previewRends)
            {
                r.enabled = true;
            }
        }
    }

    /// <summary>Move the preview ghost.</summary>
    public void SetPreviewPose(Vector3 worldPos, Quaternion worldRot)
    {
        if (!isPreviewing) return;

        if (usePreviewClone)
        {
            if (previewInstance == null) return;
            previewInstance.transform.SetPositionAndRotation(worldPos, worldRot);
        }
        else
        {
            // If no clone, move the real object (fallback)
            transform.SetPositionAndRotation(worldPos, worldRot);
        }
    }

    public void UpdatePreviewMaterial(bool canPlace)
    {
        if (!isPreviewing) return;

        if (previewValidMat == null || previewInvalidMat == null) return;

        Material target = canPlace ? previewValidMat : previewInvalidMat;

        if (usePreviewClone)
        {
            if (previewRends == null) return;
            for (int i = 0; i < previewRends.Length; i++)
                previewRends[i].sharedMaterial = target;
        }
        else
        {
            if (rends == null) return;
            for (int i = 0; i < rends.Length; i++)
                rends[i].sharedMaterial = target;
        }
    }

    /// <summary>
    /// Finalize placement:
    /// - Deletes preview ghost
    /// - Moves REAL suitcase into bay
    /// - Re-enables real visuals + colliders
    /// - Applies placed material
    /// </summary>
    public void CommitPlaced_MoveReal_DeletePreview(Vector3 finalPos, Quaternion finalRot, Transform parentUnderBay)
    {
        isPreviewing = false;

        // Destroy ghost
        DestroyPreviewOnly();

        // Move the REAL suitcase
        transform.SetPositionAndRotation(finalPos, finalRot);

        if (parentUnderBay != null)
            transform.SetParent(parentUnderBay, true);

        // Show real suitcase now
        SetRenderersEnabled(true);

        // Apply placed material
        if (placedMat != null && rends != null)
        {
            for (int i = 0; i < rends.Length; i++)
                rends[i].sharedMaterial = placedMat;
        }

        // Make it solid
        SetCollidersEnabled(true);

        // Keep it grid-solid (no physics)
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public void CancelPreview()
    {
        isPreviewing = false;

        DestroyPreviewOnly();

        // Put the real suitcase back to visible + collidable
        SetRenderersEnabled(true);
        SetCollidersEnabled(true);

        // If you want physics back on cancel:
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

    private void DestroyPreviewOnly()
    {
        if (previewInstance != null)
        {
            Destroy(previewInstance);
            previewInstance = null;
            previewRends = null;
        }
    }

    public void SetCollidersEnabled(bool enabled)
    {
        if (cols == null) return;
        for (int i = 0; i < cols.Length; i++)
            if (cols[i] != null) cols[i].enabled = enabled;
    }

    private void SetRenderersEnabled(bool enabled)
    {
        if (rends == null) return;
        for (int i = 0; i < rends.Length; i++)
            if (rends[i] != null) rends[i].enabled = enabled;
    }

    public Vector3Int[] GetShapeCells()
    {
        var shape = GetComponent<SuitcaseShape>();
        return shape != null ? shape.Cells : new[] { Vector3Int.zero };
    }
}