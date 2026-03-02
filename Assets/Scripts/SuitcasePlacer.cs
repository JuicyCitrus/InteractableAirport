using UnityEngine;
using UnityEngine.InputSystem;

public class SuitcasePlacer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CargoBayGrid bayGrid;
    [SerializeField] private Camera playerCam;
    [SerializeField] private SuitcaseQueueManager queueManager;

    [Header("No-Bonk Sampling")]
    [SerializeField] private bool useRayEndSampling = true;
    [SerializeField] private float selectSearchRadius = 0.75f;
    [SerializeField] private float nodeSearchRadius = 0.75f;

    [Header("Raycast - Suitcase Selection")]
    [SerializeField] private LayerMask suitcaseMask = ~0;
    [SerializeField] private float selectDistance = 4f;

    [Header("Raycast - Bay Placement (Nodes)")]
    [SerializeField] private LayerMask bayMask = ~0; // BayNode layer
    [SerializeField] private float placeDistance = 6f;

    [Header("Preview Smoothing")]
    [SerializeField] private float previewLerpSpeed = 18f;

    [Header("Placement Rules")]
    [SerializeField] private bool requireSupportBelow = true;

    // Active suitcase (real instance)
    private SuitcaseItem activeSuitcase;

    // Sticky hover
    private bool hasLastCell;
    private Vector3Int lastCell;
    private bool canPlaceHere;

    // Lerp state (world space)
    private Vector3 previewPos;
    private bool previewPosInitialized;

    // Input
    private Controls controls;

    // Non-alloc buffer
    private readonly Collider[] overlapBuffer = new Collider[96];

    private void Awake()
    {
        if (playerCam == null) playerCam = Camera.main;
        controls = new Controls();

        if (bayGrid != null)
        {
            if (nodeSearchRadius <= 0f) nodeSearchRadius = bayGrid.cellSize * 0.75f;
            if (selectSearchRadius <= 0f) selectSearchRadius = bayGrid.cellSize * 0.75f;
        }
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Attack.started += OnAttackStarted;
    }

    private void OnDisable()
    {
        if (controls != null)
            controls.Player.Attack.started -= OnAttackStarted;

        controls.Disable();
    }

    private void Update()
    {
        if (activeSuitcase == null) return;

        UpdateHoverCell_Node_Sticky_NoBonk();
        UpdatePreviewVisual_StickyLerped();
    }

    private void OnAttackStarted(InputAction.CallbackContext ctx)
    {
        if (activeSuitcase == null)
        {
            TrySelectSuitcase();
            return;
        }

        TryPlace();
    }

    // =========================
    // Suitcase Selection (Queue-first)
    // =========================
    private void TrySelectSuitcase()
    {
        if (playerCam == null) return;

        // 1) Figure out what suitcase the player is actually aiming at (no-bonk)
        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        SuitcaseItem aimedItem = null;

        if (useRayEndSampling)
        {
            Vector3 samplePoint = ray.origin + ray.direction * selectDistance;
            aimedItem = FindClosestSuitcase(samplePoint);
        }
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hit, selectDistance, suitcaseMask, QueryTriggerInteraction.Collide))
                aimedItem = hit.collider.GetComponentInParent<SuitcaseItem>();
        }

        if (aimedItem == null || !aimedItem.enabled)
            return;

        // 2) If we have a queue manager, ONLY allow selecting the current pickup suitcase
        if (queueManager != null)
        {
            SuitcaseItem pickup = queueManager.GetCurrentPickupSuitcase();

            // Must be aiming at the exact lead suitcase
            if (pickup == null || pickup != aimedItem || !pickup.enabled)
                return;

            activeSuitcase = pickup;
            activeSuitcase.BeginPreview();

            queueManager.NotifyPickedUp(activeSuitcase);
            OnSelectedSuitcase();
            return;
        }

        // 3) No queue manager: allow free pick of whatever they aimed at
        activeSuitcase = aimedItem;
        activeSuitcase.BeginPreview();
        OnSelectedSuitcase();
    }

    private void OnSelectedSuitcase()
    {
        hasLastCell = false;
        previewPosInitialized = false;

        UpdateHoverCell_Node_Sticky_NoBonk();
        UpdatePreviewVisual_StickyLerped();
    }

    private SuitcaseItem FindClosestSuitcase(Vector3 samplePoint)
    {
        int count = Physics.OverlapSphereNonAlloc(
            samplePoint,
            selectSearchRadius,
            overlapBuffer,
            suitcaseMask,
            QueryTriggerInteraction.Collide
        );

        if (count <= 0) return null;

        SuitcaseItem best = null;
        float bestSqr = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            Collider c = overlapBuffer[i];
            if (c == null) continue;

            SuitcaseItem item = c.GetComponentInParent<SuitcaseItem>();
            if (item == null || !item.enabled) continue; // ✅ ignore placed/disabled

            float sqr = (item.transform.position - samplePoint).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = item;
            }
        }

        return best;
    }

    // =========================
    // Bay Node Hover (No Bonk + Sticky)
    // =========================
    private void UpdateHoverCell_Node_Sticky_NoBonk()
    {
        canPlaceHere = false;

        if (bayGrid == null || playerCam == null) return;

        Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);

        CargoBayNode node = null;

        if (useRayEndSampling)
        {
            Vector3 samplePoint = ray.origin + ray.direction * placeDistance;
            node = FindClosestNode(samplePoint);
        }
        else
        {
            if (Physics.Raycast(ray, out RaycastHit hit, placeDistance, bayMask, QueryTriggerInteraction.Collide))
            {
                node = hit.collider.GetComponent<CargoBayNode>();
                if (node == null) node = hit.collider.GetComponentInParent<CargoBayNode>();
            }
        }

        if (node != null)
        {
            lastCell = node.Cell;
            hasLastCell = true;
            canPlaceHere = ComputeCanPlaceAt(lastCell);
        }
        else
        {
            if (hasLastCell)
                canPlaceHere = ComputeCanPlaceAt(lastCell);
        }
    }

    private CargoBayNode FindClosestNode(Vector3 samplePoint)
    {
        int count = Physics.OverlapSphereNonAlloc(
            samplePoint,
            nodeSearchRadius,
            overlapBuffer,
            bayMask,
            QueryTriggerInteraction.Collide
        );

        if (count <= 0) return null;

        CargoBayNode best = null;
        float bestSqr = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            Collider c = overlapBuffer[i];
            if (c == null) continue;

            CargoBayNode node = c.GetComponent<CargoBayNode>();
            if (node == null) node = c.GetComponentInParent<CargoBayNode>();
            if (node == null) continue;

            float sqr = (node.transform.position - samplePoint).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = node;
            }
        }

        return best;
    }

    // =========================
    // Placement validation (multi-cell aware)
    // =========================
    private bool ComputeCanPlaceAt(Vector3Int originCell)
    {
        if (bayGrid == null || activeSuitcase == null) return false;

        Vector3Int[] cells = activeSuitcase.GetShapeCells();

        // 1) Fit + no overlap
        if (!bayGrid.CanPlace(cells, originCell))
            return false;

        // 2) Support rule (no floating)
        if (requireSupportBelow)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3Int c = originCell + cells[i];
                Vector3Int below = c + Vector3Int.down;

                // If piece has a cell directly below, this is not a bottom-facing cell.
                bool hasPieceBelow = false;
                for (int j = 0; j < cells.Length; j++)
                {
                    if (originCell + cells[j] == below)
                    {
                        hasPieceBelow = true;
                        break;
                    }
                }
                if (hasPieceBelow) continue;

                if (c.y == 0) continue;

                if (!bayGrid.InBounds(below)) return false;
                if (!bayGrid.IsOccupied(below)) return false;
            }
        }

        return true;
    }

    // =========================
    // Preview visuals (Sticky + Lerped)
    // =========================
    private void UpdatePreviewVisual_StickyLerped()
    {
        if (activeSuitcase == null) return;
        if (!hasLastCell) return;

        Vector3 targetPos = bayGrid.CellToWorldCenter(lastCell);
        Quaternion targetRot = Quaternion.identity;

        if (!previewPosInitialized)
        {
            previewPos = targetPos;
            previewPosInitialized = true;
        }

        float t = 1f - Mathf.Exp(-previewLerpSpeed * Time.deltaTime);
        previewPos = Vector3.Lerp(previewPos, targetPos, t);

        // Still using the preview helper on SuitcaseItem
        activeSuitcase.SetPreviewPose(previewPos, targetRot);
        activeSuitcase.UpdatePreviewMaterial(canPlaceHere);
    }

    // =========================
    // Place (move real suitcase + delete preview + disable script)
    // =========================
    private void TryPlace()
    {
        if (activeSuitcase == null) return;
        if (!hasLastCell || !canPlaceHere) return;
        if (bayGrid == null) return;

        Vector3Int[] cells = activeSuitcase.GetShapeCells();

        bool placed = bayGrid.Place(cells, lastCell);
        if (!placed) return;

        Vector3 finalPos = bayGrid.CellToWorldCenter(lastCell);
        Quaternion finalRot = Quaternion.identity;

        activeSuitcase.CommitPlaced_MoveReal_DeletePreview(finalPos, finalRot, bayGrid.transform);

        activeSuitcase.enabled = false;

        activeSuitcase = null;
        hasLastCell = false;
        canPlaceHere = false;
        previewPosInitialized = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (playerCam == null) return;

        Gizmos.color = new Color(1f, 1f, 0f, 0.6f);

        Vector3 selectP = playerCam.transform.position + playerCam.transform.forward * selectDistance;
        Gizmos.DrawWireSphere(selectP, selectSearchRadius);

        Vector3 placeP = playerCam.transform.position + playerCam.transform.forward * placeDistance;
        Gizmos.DrawWireSphere(placeP, nodeSearchRadius);
    }
#endif
}