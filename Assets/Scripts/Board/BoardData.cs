using UnityEngine;

[CreateAssetMenu(fileName = "Board Data", menuName = "Board Data")]
public class BoardData : ScriptableObject
{
    public Vector2Int BoardSize = new Vector2Int(10, 22);
    [Range(0f, 1f)] public float ProjectionAlpha = 0.3f;
}
