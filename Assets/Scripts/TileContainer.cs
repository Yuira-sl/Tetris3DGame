using UnityEngine;

public class TileContainer: MonoBehaviour
{
    [SerializeField] private Vector3[] _positions;
    public Vector3[] Positions => _positions;
}