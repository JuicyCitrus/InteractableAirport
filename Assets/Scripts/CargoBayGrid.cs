using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CargoBayGrid : MonoBehaviour
{
    [Header("Grid Size (cells)")]
    [Min(1)] public int width = 8;   // X
    [Min(1)] public int height = 5;  // Y
    [Min(1)] public int depth = 10;  // Z

    [Header("Cell Settings")]
    [Min(0.1f)] public float cellSize = 1f;

    [Tooltip("Optional: If set, the grid volume will be derived from this BoxCollider (center/size) in local space.")]
    public BoxCollider volumeCollider;

    [Header("Nodes (Ray Target)")]
    public bool useNodeColliders = true;
    public bool disableVolumeColliderWhenUsingNodes = true;

    [Tooltip("Layer to assign to node GameObjects (e.g. BayNode). Set to -1 to leave unchanged.")]
    public int nodeLayer = -1;

    [Header("Node Collider Settings")]
    [Range(0.1f, 1f)]
    public float nodeSizeMultiplier = 0.33f;

    [Tooltip("If true, hides BayNodes + node children from the Hierarchy (runtime only).")]
    public bool hideRuntimeNodesInHierarchy = true;

    private CargoBayNode[,,] nodes;
    private Transform nodeRoot;

    [Header("Debug")]
    public bool drawGizmos = true;
    public bool drawOccupied = true;

    // occupied[x,y,z]
    private bool[,,] occupied;

    // Cached derived values
    private Vector3 localOrigin; // local-space min corner of the grid (cell 0,0,0)
    private Vector3 localSize;   // local-space total size of grid in units

    private void Awake()
    {
        Rebuild();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        width = Mathf.Max(1, width);
        height = Mathf.Max(1, height);
        depth = Mathf.Max(1, depth);
        cellSize = Mathf.Max(0.1f, cellSize);

        // IMPORTANT: Do NOT spawn node colliders in edit mode.
        // Just keep cached values updated so gizmos match.
        if (!Application.isPlaying)
        {
            RecomputeLocalBoundsOnly();
        }
    }
#endif

    /// <summary>
    /// Re-allocates occupancy and recalculates derived grid bounds.
    /// Call this if you change size at runtime.
    /// </summary>
    public void Rebuild()
    {
        occupied = new bool[width, height, depth];

        RecomputeLocalBoundsOnly();

        // Only build nodes in PLAY MODE (prevents duplicates / clutter in editor)
        if (Application.isPlaying && useNodeColliders)
        {
            BuildNodesRuntimeOnly();
        }
        else
        {
            // Ensure big volume collider isn't accidentally disabled in editor
            if (!Application.isPlaying && volumeCollider != null)
                volumeCollider.enabled = true;
        }
    }

    private void RecomputeLocalBoundsOnly()
    {
        if (volumeCollider != null)
        {
            var c = volumeCollider.center;
            var s = volumeCollider.size;
            localOrigin = c - (s * 0.5f);
        }
        else
        {
            localOrigin = Vector3.zero;
        }

        localSize = new Vector3(width * cellSize, height * cellSize, depth * cellSize);
    }

    private void BuildNodesRuntimeOnly()
    {
        // Cleanup old immediately (runtime-safe)
        if (nodeRoot != null)
        {
            Destroy(nodeRoot.gameObject);
            nodeRoot = null;
        }

        nodeRoot = new GameObject("BayNodes").transform;
        nodeRoot.SetParent(transform, false);

        if (nodeLayer >= 0)
            nodeRoot.gameObject.layer = nodeLayer;

        if (hideRuntimeNodesInHierarchy)
            nodeRoot.gameObject.hideFlags = HideFlags.HideInHierarchy;

        nodes = new CargoBayNode[width, height, depth];

        // Disable big collider so it doesn't steal ray hits
        if (disableVolumeColliderWhenUsingNodes && volumeCollider != null)
            volumeCollider.enabled = false;

        float triggerSize = cellSize * nodeSizeMultiplier;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                {
                    var go = new GameObject();

                    if (nodeLayer >= 0)
                        go.layer = nodeLayer;

                    if (hideRuntimeNodesInHierarchy)
                        go.hideFlags = HideFlags.HideInHierarchy;

                    go.transform.SetParent(nodeRoot, false);

                    // Put at cell center in LOCAL space
                    Vector3 localPos = localOrigin + new Vector3(
                        (x + 0.5f) * cellSize,
                        (y + 0.5f) * cellSize,
                        (z + 0.5f) * cellSize
                    );
                    go.transform.localPosition = localPos;

                    var col = go.AddComponent<BoxCollider>();
                    col.isTrigger = true;
                    col.size = Vector3.one * triggerSize;

                    var node = go.AddComponent<CargoBayNode>();
                    node.Init(new Vector3Int(x, y, z));
                    nodes[x, y, z] = node;
                }
    }

    public bool InBounds(Vector3Int cell)
    {
        return cell.x >= 0 && cell.x < width
            && cell.y >= 0 && cell.y < height
            && cell.z >= 0 && cell.z < depth;
    }

    public bool IsOccupied(Vector3Int cell)
    {
        if (!InBounds(cell)) return true;
        return occupied[cell.x, cell.y, cell.z];
    }

    public void SetOccupied(Vector3Int cell, bool value)
    {
        if (!InBounds(cell))
        {
            Debug.LogWarning($"[CargoBayGrid] Tried to SetOccupied out of bounds: {cell}");
            return;
        }
        occupied[cell.x, cell.y, cell.z] = value;
    }

    public void ClearAll()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                    occupied[x, y, z] = false;
    }

    public Vector3 CellToWorldCenter(Vector3Int cell)
    {
        Vector3 local = localOrigin + new Vector3(
            (cell.x + 0.5f) * cellSize,
            (cell.y + 0.5f) * cellSize,
            (cell.z + 0.5f) * cellSize
        );
        return transform.TransformPoint(local);
    }

    public bool WorldToCell(Vector3 worldPos, out Vector3Int cell)
    {
        Vector3 local = transform.InverseTransformPoint(worldPos);
        Vector3 rel = local - localOrigin;

        int x = Mathf.FloorToInt(rel.x / cellSize);
        int y = Mathf.FloorToInt(rel.y / cellSize);
        int z = Mathf.FloorToInt(rel.z / cellSize);

        cell = new Vector3Int(x, y, z);
        return InBounds(cell);
    }

    public bool CanPlace(IReadOnlyList<Vector3Int> pieceLocalCells, Vector3Int originCell)
    {
        if (pieceLocalCells == null || pieceLocalCells.Count == 0)
            return false;

        for (int i = 0; i < pieceLocalCells.Count; i++)
        {
            Vector3Int c = originCell + pieceLocalCells[i];
            if (!InBounds(c)) return false;
            if (occupied[c.x, c.y, c.z]) return false;
        }
        return true;
    }

    public bool Place(IReadOnlyList<Vector3Int> pieceLocalCells, Vector3Int originCell)
    {
        if (!CanPlace(pieceLocalCells, originCell))
            return false;

        for (int i = 0; i < pieceLocalCells.Count; i++)
        {
            Vector3Int c = originCell + pieceLocalCells[i];
            occupied[c.x, c.y, c.z] = true;
        }
        return true;
    }

    public void Unplace(IReadOnlyList<Vector3Int> pieceLocalCells, Vector3Int originCell)
    {
        if (pieceLocalCells == null) return;

        for (int i = 0; i < pieceLocalCells.Count; i++)
        {
            Vector3Int c = originCell + pieceLocalCells[i];
            if (!InBounds(c)) continue;
            occupied[c.x, c.y, c.z] = false;
        }
    }

    public Bounds GetWorldBounds()
    {
        Vector3 localCenter = localOrigin + localSize * 0.5f;
        Vector3 worldCenter = transform.TransformPoint(localCenter);

        float sx = transform.lossyScale.x;
        float sy = transform.lossyScale.y;
        float sz = transform.lossyScale.z;

        Vector3 worldSize = new Vector3(localSize.x * Mathf.Abs(sx), localSize.y * Mathf.Abs(sy), localSize.z * Mathf.Abs(sz));
        return new Bounds(worldCenter, worldSize);
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;
        if (width <= 0 || height <= 0 || depth <= 0 || cellSize <= 0) return;

        // Compute derived in editor safely (so gizmos always correct)
        Vector3 origin;
        if (volumeCollider != null)
            origin = volumeCollider.center - (volumeCollider.size * 0.5f);
        else
            origin = Vector3.zero;

        Vector3 size = new Vector3(width * cellSize, height * cellSize, depth * cellSize);
        Vector3 localCenter = origin + size * 0.5f;

        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

        Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
        Gizmos.DrawWireCube(localCenter, size);

        Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.35f);
        float r = cellSize * 0.08f;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                for (int z = 0; z < depth; z++)
                {
                    Vector3 cellCenterLocal = origin + new Vector3(
                        (x + 0.5f) * cellSize,
                        (y + 0.5f) * cellSize,
                        (z + 0.5f) * cellSize
                    );
                    Gizmos.DrawSphere(cellCenterLocal, r);
                }

        if (drawOccupied && occupied != null)
        {
            Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.35f);
            Vector3 cellDrawSize = Vector3.one * cellSize * 0.95f;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    for (int z = 0; z < depth; z++)
                    {
                        if (!occupied[x, y, z]) continue;

                        Vector3 cellCenterLocal = origin + new Vector3(
                            (x + 0.5f) * cellSize,
                            (y + 0.5f) * cellSize,
                            (z + 0.5f) * cellSize
                        );

                        Gizmos.DrawCube(cellCenterLocal, cellDrawSize);
                    }
        }

        Gizmos.matrix = Matrix4x4.identity;
    }
}