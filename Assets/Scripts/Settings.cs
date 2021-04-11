using System;
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
    
    [Header("Grid")]
    public Vector3Int GridSize = new Vector3Int(5, 8, 5);
    public Vector3Int SpawnPointer = new Vector3Int(2, 7, 2);
    
    [Header("Camera")]
    public float RotationSpeed = 1f;
    public float CameraDistance = 7f;
    public Vector2 CameraDistanceMinMax = new Vector2(6f, 18f);
    public float ZoomSpeed = 4f;

    [Header("Dropping Settings & Time")]
    public float CurrentDropTime = 1f;
    public float DropTimeDefault = 1f;
    public Vector2 DropTimeMinMax = new Vector2(0.4f, 1f);
    public float DifficultyTime = 10f;
    public float ClearTime = 0.1f;
    
    [Header("Input Controls")]
    public KeyCode ForwardBtn = KeyCode.W;
    public KeyCode BackwardBtn = KeyCode.S;
    public KeyCode LeftBtn = KeyCode.A;
    public KeyCode RightBtn = KeyCode.D;
    public KeyCode RotateXZBtn = KeyCode.Alpha1;
    public KeyCode RotateXYBtn = KeyCode.Alpha2;
    public KeyCode RotateYZBtn = KeyCode.Alpha3;
    public KeyCode DropBtn = KeyCode.Space;
    public KeyCode PauseBtn = KeyCode.Escape;
    public KeyCode RotateCamera = KeyCode.Mouse0;
    
   
    public GameObject[,,] TileLiterals;
    public GameObject[,,] GhostLiterals;
    public bool[,,] IsCellFilled;
    public PieceController CurrentPieceController;
    [HideInInspector] public float DifficultyClock;
    [HideInInspector] public float ClearClock;
    [HideInInspector] public float DropClock;
    [HideInInspector] public bool XZClockwise;
    [HideInInspector] public bool XYClockwise;
    [HideInInspector] public bool YZClockwise;
    [HideInInspector] public float CameraAngleXY;
    [HideInInspector] public float CameraAngleXZ;
    [HideInInspector] public float LastCameraAngleXY;
    [HideInInspector] public float LastCameraAngleXZ;
    [HideInInspector] public Vector3 CameraTarget;
    [HideInInspector] public Vector3 LastCameraTarget;
    [HideInInspector] public bool Clearing;
    [HideInInspector] public bool[] ClearingLevels;
    [HideInInspector] public bool FreeSpin = true;
    [HideInInspector] public bool NeedsUpdate;
    [HideInInspector] public bool Paused;
}