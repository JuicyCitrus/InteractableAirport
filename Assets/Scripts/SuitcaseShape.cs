using UnityEngine;

public class SuitcaseShape : MonoBehaviour
{
    [Tooltip("Cells occupied by this suitcase, in local grid offsets from the origin cell (0,0,0).")]
    public Vector3Int[] cells = new Vector3Int[] { Vector3Int.zero };

    private Vector3Int[][] cached = new Vector3Int[4][];

    public Vector3Int[] Cells => (cells != null && cells.Length > 0) ? cells : new[] { Vector3Int.zero };

    private void Awake() => RebuildCache();

#if UNITY_EDITOR
    private void OnValidate() => RebuildCache();
#endif

    public Vector3Int[] GetCellsForRotation(int rotStepsCW)
    {
        rotStepsCW = Mod4(rotStepsCW);
        if (cached[rotStepsCW] == null || cached[rotStepsCW].Length == 0)
            RebuildCache();
        return cached[rotStepsCW];
    }

    private void RebuildCache()
    {
        var baseCells = Cells;

        cached[0] = Copy(baseCells);
        cached[1] = RotateCW90(cached[0]);
        cached[2] = RotateCW90(cached[1]);
        cached[3] = RotateCW90(cached[2]);
    }

    private static Vector3Int[] RotateCW90(Vector3Int[] src)
    {
        // (x,z) -> (z, -x), y unchanged (CW looking down +Y)
        var dst = new Vector3Int[src.Length];
        for (int i = 0; i < src.Length; i++)
        {
            var c = src[i];
            dst[i] = new Vector3Int(c.z, c.y, -c.x);
        }
        return dst;
    }

    private static Vector3Int[] Copy(Vector3Int[] src)
    {
        var dst = new Vector3Int[src.Length];
        for (int i = 0; i < src.Length; i++) dst[i] = src[i];
        return dst;
    }

    private static int Mod4(int v)
    {
        v %= 4;
        if (v < 0) v += 4;
        return v;
    }
}