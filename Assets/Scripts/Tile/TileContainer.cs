using UnityEngine;

public class TileContainer: MonoBehaviour
{
    [SerializeField] private Transform _pivot;
    [SerializeField] private Vector3[] _positions;

    public Transform Pivot => _pivot;
    public Vector3[] Positions => _positions;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var position in _positions)
        {
            Gizmos.DrawWireCube(position, Vector3.one);
        }

        if (_pivot != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_pivot.position, 0.1f);
        }
    }
#endif
}