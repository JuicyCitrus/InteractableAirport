using UnityEngine;

public class CargoBayNode : MonoBehaviour
{
    public Vector3Int Cell { get; private set; }

    public void Init(Vector3Int cell)
    {
        Cell = cell;
        name = $"Node_{cell.x}_{cell.y}_{cell.z}";
    }
}