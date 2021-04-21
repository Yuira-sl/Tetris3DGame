using UnityEngine;

public class TileContainer: MonoBehaviour
{
    [SerializeField] private Vector3[] _positions;
    public Vector3[] Positions => _positions;

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var position in _positions)
        {
            Gizmos.DrawWireCube(position, Vector3.one);
        }
    }
    #endif
}