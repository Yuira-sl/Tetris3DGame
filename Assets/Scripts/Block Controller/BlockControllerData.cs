using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block Controller Data", menuName = "Block Controller Data")]
public class BlockControllerData : ScriptableObject
{
    public List<GameObject> BlockPool;
    public List<Material> Materials;
    public Material GhostMaterial;
    [Range(0f, 0.5f)] public float BlockTurningSpeed = 0.1f;
    public Vector2 BlockStartingPosition = new Vector2(4f, 20f);
    public Vector3 NextBlockScale = new Vector3(0.35f, 0.35f, 0.35f);
}
