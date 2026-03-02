using UnityEngine;

public class SuitcaseShape : MonoBehaviour
{
    [Tooltip("Cells occupied by this suitcase, in local grid offsets from the origin cell.")]
    public Vector3Int[] cells = new Vector3Int[] { Vector3Int.zero };

    public Vector3Int[] Cells => cells != null && cells.Length > 0 ? cells : new[] { Vector3Int.zero };

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 1f, 1f, 0.8f);
        for (int i = 0; i < Cells.Length; i++)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)Cells[i] * 0.25f, Vector3.one * 0.2f);
        }
    }
#endif
}