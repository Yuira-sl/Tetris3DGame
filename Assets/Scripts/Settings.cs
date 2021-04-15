using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings Preset", menuName = "Settings")]
public class Settings : ScriptableObject
{
    [Header("Assets")]
    public GameObject Block;
    public GameObject GhostBlock;
    public List<Material> BlockMaterials = new List<Material>();
    public GameObject BasePlateSource;
    public ParticleSystem ClearEffectSource;
    public GameObject GodRays;
    
    [Header("Grid")]
    public Vector2Int GridSize = new Vector2Int(5, 8);
    public Vector2Int SpawnPointer = new Vector2Int(2, 7);
  
    [Header("Dropping Settings & Time")]
    public float CurrentDropTime = 1f;
    public float DropTimeDefault = 1f;
    public Vector2 DropTimeMinMax = new Vector2(0.4f, 1f);
    public float DifficultyTime = 10f;
    public float ClearTime = 0.1f;
    
    public GameObject[,] TileLiterals;
    public GameObject[,] GhostLiterals;
    public bool[,] IsCellFilled;
    public PieceController CurrentPieceController;
    [HideInInspector] public float DifficultyClock;
    [HideInInspector] public float ClearClock;
    [HideInInspector] public float DropClock;
    [HideInInspector] public bool Clearing;
    [HideInInspector] public bool[] ClearingLevels;
    [HideInInspector] public bool NeedsUpdate;
    [HideInInspector] public bool Paused;
}